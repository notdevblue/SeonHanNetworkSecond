const express = require('express');
const fs = require('fs/promises');
const http = require('http');
const path = require('path');

const app = express();

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

server.listen(54000, ()=>{
    console.log("서버가 54000번 포트에서 구동중입니다.");
});

