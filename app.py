import os
import json
import pyodbc
from flask import Flask, render_template, jsonify, request, session, redirect, url_for
from functools import wraps

app = Flask(__name__)
app.secret_key = "tctool-secret-2024"

# ── DB config ─────────────────────────────────────────────────────────────────
DB_USER      = "TCtoolUser"
DB_SERVER    = "eu2-dev-taxcaddy-sqlsrv.database.windows.net"
DB_NAME      = "eu2-dev-Log-sql-db"
DB_PORT      = "1433"
APP_PASSWORD = "tctool2024"   # login password

def get_connection():
    db_password = session.get("db_password")
    if not db_password:
        raise ValueError("DB password not set in session.")
    conn_str = (
        f"DRIVER={{ODBC Driver 18 for SQL Server}};"
        f"SERVER={DB_SERVER},{DB_PORT};"
        f"DATABASE={DB_NAME};"
        f"UID={DB_USER};"
        f"PWD={db_password};"
        f"Encrypt=yes;"
        f"TrustServerCertificate=no;"
        f"Connection Timeout=30;"
    )
    return pyodbc.connect(conn_str)

# ── Auth helpers ───────────────────────────────────────────────────────────────
def login_required(f):
    @wraps(f)
    def decorated(*args, **kwargs):
        if not session.get("logged_in"):
            return redirect(url_for("login"))
        return f(*args, **kwargs)
    return decorated

# ── Routes ────────────────────────────────────────────────────────────────────
@app.route("/login", methods=["GET", "POST"])
def login():
    error = None
    if request.method == "POST":
        pwd = request.form.get("password", "")
        db_pwd = request.form.get("db_password", "")
        if pwd == APP_PASSWORD:
            if not db_pwd:
                error = "Please enter the database password."
            else:
                session["logged_in"] = True
                session["db_password"] = db_pwd
                return redirect(url_for("index"))
        else:
            error = "Incorrect password. Please try again."
    return render_template("login.html", error=error)

@app.route("/logout")
def logout():
    session.clear()
    return redirect(url_for("login"))

@app.route("/")
@login_required
def index():
    return render_template("index.html")

@app.route("/api/tctoolconfig")
@login_required
def get_tctoolconfig():
    try:
        conn = get_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM TCToolConfig")
        columns = [col[0] for col in cursor.description]
        rows = []
        for row in cursor.fetchall():
            rows.append(dict(zip(columns, [
                str(v) if not isinstance(v, (int, float, bool, type(None))) else v
                for v in row
            ])))
        cursor.close()
        conn.close()
        return jsonify({"success": True, "count": len(rows), "data": rows})
    except Exception as e:
        return jsonify({"success": False, "error": str(e)}), 500

@app.route("/api/tctoolconfig/count")
@login_required
def get_count():
    try:
        conn = get_connection()
        cursor = conn.cursor()
        cursor.execute("SELECT COUNT(*) FROM TCToolConfig")
        count = cursor.fetchone()[0]
        cursor.close()
        conn.close()
        return jsonify({"success": True, "count": count})
    except Exception as e:
        return jsonify({"success": False, "error": str(e)}), 500

if __name__ == "__main__":
    port = int(os.getenv("PORT", 5000))
    app.run(host="0.0.0.0", port=port, debug=False)
