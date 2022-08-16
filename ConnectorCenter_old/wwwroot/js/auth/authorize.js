function authorize(login, password) {
    var tokenKey = "token";
    // отправляет запрос и получаем ответ
    const response = await fetch("/api/authorization/authorize", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({

            Login: document.getElementById("login").value,
            Password: document.getElementById("password").value
        })
    });
    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const data = await response.json();
        // сохраняем в хранилище sessionStorage токен доступа
        sessionStorage.setItem(tokenKey, data.token);
        location.assign("/dashboard", {
            Headers: { "Authorization": "Bearer " + tokenKey }
        });
    }
    else {
        console.log("Authorize status: ", response.status);
        location.assign("/login", {
            Headers: { "Authorization": "Bearer " + tokenKey }
        });
    }
}

function signout() {
    sessionStorage.removeItem("token");
}
