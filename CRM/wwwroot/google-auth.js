document.getElementById('google-sign-out-btn').addEventListener("click", () => signOut());
document.getElementById('test-btn').addEventListener("click", () => authTest());

function signOut() {
    document.getElementById('account-info').innerHTML = "";
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        document.getElementById('user-info').innerHTML = "No authorized";
    });
}; 

function onSignIn(googleUser) {
    document.getElementById('account-info').innerHTML = "";
    var profile = googleUser.getBasicProfile();
    var id_token = googleUser.getAuthResponse().id_token;
    document.getElementById('user-info').innerHTML = "Hello, " + profile.getName() + "!";

    postToken(id_token, profile);
}

function authTest() {
    fetch('https://localhost:5100/api/lead/266')
        .then((response) => response.json())
        .then((data) => {
            Console.log(data);
        });
}

async function postToken(token, userInfo) {
    let data = {
        token: token,
        username: userInfo.getName(),
        email: userInfo.getEmail()
    };
    console.log(JSON.stringify({ "email": data.email }));

    var result = await fetch('https://localhost:5100/api/token/google-token', {
        method: 'post',
        body: JSON.stringify(data),
        headers: { 'Content-type': 'application/json' }
    })
    .then(response => {
        if (!response.ok) {
            document.getElementById('account-info').innerHTML = "User not found";
        }
        else {
            return response.json();
        }
    })
        
    if (result) {
        var resultStr = "Token: " + result.access_token + "<br><br>";
        for (i = 0; i < result.accountsInfo.length; i++) {
            resultStr += "<input type='radio' id='account" + i+1 +
                "' name='accountId' value='account" + i+1 + "'>" + "<label for='account" + i+1 + "'>" + 
                "AccountId = " + result.accountsInfo[i].id + ", " +
                "Amount = " + result.accountsAmount[i] + ", " +
                "Currency = " + result.accountsInfo[i].currency.code + "</label><br>";
        };
        resultStr +="<br>"
        document.getElementById('account-info').innerHTML = resultStr;
    }
}
