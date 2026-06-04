# TCTool Config Dashboard (Python/Flask)

A secure web dashboard to view data from `TCToolConfig` on Azure SQL.
Built with Python, Flask, and pyodbc.

---

## Run Locally

```bash
# 1. Install dependencies
pip install -r requirements.txt

# Install ODBC Driver 18 (if not installed):
# Windows: https://aka.ms/odbc18
# Ubuntu:  sudo apt-get install msodbcsql18

# 2. Start the app
python app.py

# 3. Open browser
# http://localhost:5000
# Password: tctool2024 (set APP_PASSWORD in .env)
```

---

## Deploy to Railway (Recommended — Free)

1. Push this folder to a **GitHub repo**
2. Go to [railway.app](https://railway.app) → **New Project** → **Deploy from GitHub**
3. Select your repo
4. Go to **Variables** tab and add:
   ```
   DB_USER=TCtoolUser
   DB_PASSWORD=MyUser#123456TR
   DB_SERVER=eu2-dev-taxcaddy-sqlsrv.database.windows.net
   DB_NAME=eu2-dev-Log-sql-db
   DB_PORT=1433
   APP_PASSWORD=YourSecurePassword
   SECRET_KEY=some-long-random-string
   ```
5. Railway auto-deploys and gives you a URL like `https://tctool.up.railway.app`

---

## Deploy to Render (Free)

1. Push to GitHub
2. Go to [render.com](https://render.com) → **New Web Service**
3. Connect your GitHub repo
4. Set **Start Command**: `gunicorn app:app --bind 0.0.0.0:$PORT`
5. Add the same environment variables as above
6. Deploy → get a public URL

---

## Deploy to Azure App Service (Best for Azure SQL)

```bash
az webapp up \
  --name tctool-dashboard \
  --resource-group your-rg \
  --runtime PYTHON:3.12 \
  --sku B1
```

Then set env vars via Azure Portal → App Service → Configuration.

---

## Files

```
tctool_python/
├── app.py              ← Flask backend + API routes + login
├── templates/
│   ├── login.html      ← Password login page
│   └── index.html      ← Main dashboard
├── requirements.txt    ← Python dependencies
├── Procfile            ← For Railway/Heroku
├── railway.toml        ← Railway config
├── .env                ← DB credentials (DO NOT commit to Git)
└── .gitignore
```

---

## Security Notes

- The `.env` file is in `.gitignore` — **never push it to GitHub**
- Set `APP_PASSWORD` and `SECRET_KEY` as environment variables on your host
- All API routes require login session
- Azure SQL connection uses `Encrypt=yes` (required for Azure)
