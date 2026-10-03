# Часть 3. REST API для таблицы User (Id, Login, PassHash) с CRUD-операциями.
# Формат запросов/ответов — JSON. Пароль хранится в виде SHA-256 хеша.
# Хранилище — SQLite (файл users.db).
#
# Запуск: python app.py
#
# Эндпоинты:
#   POST   /user       — создать пользователя {"Login": "...", "PassHash": "..."}
#   GET    /user/<id>  — получить пользователя по id
#   PUT    /user/<id>  — обновить пользователя {"Login": "..."} и/или {"PassHash": "..."}
#   DELETE /user/<id>  — удалить пользователя

import hashlib
import sqlite3
from pathlib import Path

from flask import Flask, g, jsonify, request

app = Flask(__name__)
DB_PATH = Path(__file__).parent / "users.db"


def get_db():
    """Возвращает подключение к SQLite для текущего запроса (кешируется в g)."""
    if "db" not in g:
        g.db = sqlite3.connect(DB_PATH)
        g.db.row_factory = sqlite3.Row
        g.db.execute("PRAGMA foreign_keys = ON")
    return g.db


@app.teardown_appcontext
def close_db(exception=None):
    db = g.pop("db", None)
    if db is not None:
        db.close()


def init_db():
    """Создаёт таблицу User, если её ещё нет."""
    conn = sqlite3.connect(DB_PATH)
    conn.execute(
        """
        CREATE TABLE IF NOT EXISTS User (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Login TEXT NOT NULL UNIQUE,
            PassHash TEXT NOT NULL
        )
        """
    )
    conn.commit()
    conn.close()


def hash_password(raw_password: str) -> str:
    """Хеширует пароль алгоритмом SHA-256 (для учебных целей)."""
    return hashlib.sha256(raw_password.encode("utf-8")).hexdigest()


def user_to_dict(row: sqlite3.Row) -> dict:
    """Отдаём наружу Id, Login и PassHash (сам хеш, не исходный пароль)."""
    return {"Id": row["Id"], "Login": row["Login"], "PassHash": row["PassHash"]}


# ---
# POST /user — создание пользователя
# ---
@app.post("/user")
def create_user():
    data = request.get_json(silent=True)
    if not data or "Login" not in data or "PassHash" not in data:
        return jsonify({"error": "Требуется поля 'Login' и 'PassHash'"}), 400

    login = str(data["Login"]).strip()
    raw_password = str(data["PassHash"])

    if not login:
        return jsonify({"error": "Login не может быть пустым"}), 400
    if not raw_password:
        return jsonify({"error": "PassHash (пароль) не может быть пустым"}), 400

    db = get_db()

    existing = db.execute("SELECT Id FROM User WHERE Login = ?", (login,)).fetchone()
    if existing is not None:
        return jsonify({"error": f"Логин '{login}' уже занят"}), 409

    pass_hash = hash_password(raw_password)
    cursor = db.execute(
        "INSERT INTO User (Login, PassHash) VALUES (?, ?)",
        (login, pass_hash),
    )
    db.commit()

    new_row = db.execute(
        "SELECT Id, Login, PassHash FROM User WHERE Id = ?",
        (cursor.lastrowid,),
    ).fetchone()
    return jsonify(user_to_dict(new_row)), 201


# ---
# GET /user/<id> — получение пользователя по id
# ---
@app.get("/user/<int:user_id>")
def get_user(user_id: int):
    db = get_db()
    row = db.execute(
        "SELECT Id, Login, PassHash FROM User WHERE Id = ?",
        (user_id,),
    ).fetchone()
    if row is None:
        return jsonify({"error": f"Пользователь с Id={user_id} не найден"}), 404
    return jsonify(user_to_dict(row)), 200


# ---
# PUT /user/<id> — обновление пользователя
# ---
@app.put("/user/<int:user_id>")
def update_user(user_id: int):
    db = get_db()
    row = db.execute(
        "SELECT Id, Login, PassHash FROM User WHERE Id = ?",
        (user_id,),
    ).fetchone()
    if row is None:
        return jsonify({"error": f"Пользователь с Id={user_id} не найден"}), 404

    data = request.get_json(silent=True) or {}
    new_login = row["Login"]
    new_pass_hash = row["PassHash"]

    if "Login" in data:
        candidate = str(data["Login"]).strip()
        if not candidate:
            return jsonify({"error": "Login не может быть пустым"}), 400
        if candidate != row["Login"]:
            conflict = db.execute(
                "SELECT Id FROM User WHERE Login = ? AND Id != ?",
                (candidate, user_id),
            ).fetchone()
            if conflict is not None:
                return jsonify({"error": f"Логин '{candidate}' уже занят"}), 409
        new_login = candidate

    if "PassHash" in data:
        raw = str(data["PassHash"])
        if not raw:
            return jsonify({"error": "PassHash (пароль) не может быть пустым"}), 400
        new_pass_hash = hash_password(raw)

    db.execute(
        "UPDATE User SET Login = ?, PassHash = ? WHERE Id = ?",
        (new_login, new_pass_hash, user_id),
    )
    db.commit()

    updated = db.execute(
        "SELECT Id, Login, PassHash FROM User WHERE Id = ?",
        (user_id,),
    ).fetchone()
    return jsonify(user_to_dict(updated)), 200


# ---
# DELETE /user/<id> — удаление пользователя
# ---
@app.delete("/user/<int:user_id>")
def delete_user(user_id: int):
    db = get_db()
    row = db.execute("SELECT Id FROM User WHERE Id = ?", (user_id,)).fetchone()
    if row is None:
        return jsonify({"error": f"Пользователь с Id={user_id} не найден"}), 404

    db.execute("DELETE FROM User WHERE Id = ?", (user_id,))
    db.commit()
    return jsonify({"message": f"Пользователь с Id={user_id} удалён"}), 200


if __name__ == "__main__":
    init_db()
    app.run(debug=True)