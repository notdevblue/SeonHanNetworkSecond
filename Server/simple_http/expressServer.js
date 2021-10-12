const express = require('express');
const fs = require('fs/promises');
const http = require('http');
const path = require('path');
const jwt = require('jsonwebtoken');
const { pool, insertData } = require("./DB.js");
const { key } = require("./secret.js");
const app = express();

app.use(express.json()); // 요청을 json 으로 변환해주는 역할을 해준다.

//app 이 바로 요청이 왔을 때 응답을 해주는 함수야
const server = http.createServer(app);

app.get("/", (req, res) => {
    res.json({ msg: "메인페이지입니다." });
});

app.get("/hello", (req, res) => {
    res.json({ msg: "헬로 페이지입니다." });
});

app.get("/image", (req, res) => {
    let filePath = path.join(__dirname, "images", req.query.file);
    res.sendFile(filePath);
});

app.get("/record", (req, res) => {
    console.log(req.query);

    res.json({ msg: "당신의 기록 메세지로 대체되었다" });
});

app.get("/fileList", async (req, res) => {
    const files = await fs.readdir(path.join(__dirname, "images"));
    res.json({ msg: "load success", list: files });
});

app.get("/thumb", async (req, res) => {
    // /thumb/?file=r_youandme.png <= querystring 이라고 함
    let filename = req.query.file;
    let filePath = path.join(__dirname, "thumbnails", filename);

    res.sendFile(filePath);
});

app.post("/postdata", async (req, res) => {
    //console.log(req.body); // 안 됨 => 미들웨어가 필요함, post 로 보내면 body 에 data가 들어감
    let { name, msg, score } = req.body;

    const result = await insertData(name, msg, score);

    res.json({ msg: result ? true : false, list, count: list.length });

});

app.get("/list", async (req, res) => {
    
    let auth = req.headers["Authorization"];
    if (auth == undefined) {
        res.json({ result: false, payload: "로그인하세요." });
        return;
    }
    auth = auth.split(" ");
    let token = auth[1];

    if (token == undefined) {
        res.json({ result: false, payload: "잘못된 토큰입니다." });
        return;
    }

    const decode = jwt.verify(token, key); // 해당 토큰이 해당 키로 암호화 되었는지
    if (decode) {
        let sql = "SELECT * FROM high_scores ORDER BY score DESC LIMIT 0, 5"; // 최대 5개 가져옴 (MySQL => LIMIT)
        // 시작 인덱스 => 갯수
        let [list] = await pool.query(sql);
        res.json({ result: true, payload: { list, count: list.length } });
    } else {
        res.json({ result: false, payload: "잘못된 토큰입니다." });
    }

});

app.post("/register", async (req, res) => {
    let { name, id, password } = req.body;

    let sql = "SELECT id FROM users WHERE id = ?";
    const [user] = await pool.query(sql, [id]);

    if (user.length > 0) {
        res.json({ result: false, payload: "중복된 회원이 존재합니다." });
        console.log(`이미 존재하는 회원: ${id}`);
        return;
    }
    
    sql = "INSERT INTO users (id, name, password) VALUES (?, ?, PASSWORD(?))";
    await pool.query(sql, [id, name, password]);

    res.json({ result: true, payload: "성공적으로 회원가입" });
});

app.post("/login", async (req, res) => {
    let { id, password } = req.body;
    const [row] = await pool.query("SELECT * FROM users WHERE id = ? AND password = PASSWORD(?)", [id, password]);

    if (row.length > 0) {
        let { code, id, name } = row[0];
        const token = jwt.sign({ code, id, name }, key, {
            expiresIn: '30 days'
        });

        res.json({ result: true, payload: token });

    } else {
        res.json({ result: false, payload: "존재하지 않는 회원입니다." });
    }

});

server.listen(54000, () => {
    console.log("서버가 54000번 포트에서 구동중입니다.");
});

