import json

from app import app, init_db

init_db()
client = app.test_client()


def show(title, resp):
    print(f"=== {title} ===")
    print(f"Status: {resp.status_code}")
    print(json.dumps(resp.get_json(), ensure_ascii=False, indent=2))
    print("-" * 50)


r = client.post("/user", json={"Login": "alice", "PassHash": "SuperSecret123"})
show("POST /user (создание пользователя alice)", r)
alice_id = r.get_json()["Id"]

r = client.post("/user", json={"Login": "bob", "PassHash": "qwerty456"})
show("POST /user (создание пользователя bob)", r)

r = client.post("/user", json={"Login": "alice", "PassHash": "another"})
show("POST /user (повторный логин alice -> ожидаем 409)", r)

r = client.get(f"/user/{alice_id}")
show(f"GET /user/{alice_id}", r)

r = client.get("/user/999")
show("GET /user/999 (несуществующий -> 404)", r)

r = client.put(f"/user/{alice_id}", json={"PassHash": "NewPassword789"})
show(f"PUT /user/{alice_id} (меняем пароль)", r)

r = client.delete(f"/user/{alice_id}")
show(f"DELETE /user/{alice_id}", r)

r = client.get(f"/user/{alice_id}")
show(f"GET /user/{alice_id} после удаления (-> 404)", r)