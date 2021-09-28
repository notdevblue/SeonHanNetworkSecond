const express = require('express');
const fs = require('fs/promises');
const http = require('http');
const path = require('path');
const { pool, insertData } = require("./DB.js");

const app = express();

app.use(express.json()); // 요청을 json 으로 변환해주는 역할을 해준다.

//app 이 바로 요청이 왔을 때 응답을 해주는 함수야
const server = http.createServer(app);

app.get("/", (req, res)=>{
    res.json({msg:"메인페이지입니다."});
});

app.get("/hello", (req, res)=>{
    res.json({msg:"헬로 페이지입니다."});
});

app.get("/image", (req, res)=>{
    let filePath = path.join(__dirname, "images", req.query.file);
    res.sendFile(filePath);
});

app.get("/record", (req, res)=>{
    console.log(req.query);

    res.json({msg:"당신의 기록 메세지로 대체되었다"});
});

app.get("/fileList", async (req, res) => {
    const files = await fs.readdir( path.join(__dirname, "images") );
    res.json({msg:"load success", list:files });
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
    let sql = "SELECT * FROM high_scores ORDER BY score DESC LIMIT 0, 5"; // 최대 5개 가져옴 (MySQL => LIMIT)
    // 시작 인덱스 => 갯수
    let [list] = await pool.query(sql);
    
    res.json({ list });
});



server.listen(54000, ()=>{
    console.log("서버가 54000번 포트에서 구동중입니다.");
});

