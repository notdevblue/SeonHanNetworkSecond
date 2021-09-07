const http = require('http');
const fs = require('fs'); // 파일시스템
const path = require('path'); // 파일 경로

const server = http.createServer((req, res) => {
    console.log(req.url, req.method);
    // url = /something

    switch(req.url)
    {
        case "/":
            res.writeHead(200, {"Content-Type":"application/json"}); // 200 = 정상 리턴 코드, 400번대 = 에럵 // json 타입으로 보내겠다는 것
            res.end(JSON.stringify(
                {
                    msg:"Hello web server",
                    name:"gondr",
                    hobbies:[{name: "게임", id:1},
                             {name: "프로그래밍", id:1},
                             {name: "잠", id:1}]
                }));
            break;

        case "/image":
            //__dirname : 현재 폴더를 나타내는 상수
            let filePath = path.join(__dirname, "images", "youandme.png");
            // 파일시스템에서 해당 파일의 정보를 가져온다.
            let fileStat = fs.statSync(filePath);
            res.writeHead(200, {"Content-Type":"image/png", "Content-Length":fileStat.size});

            let readStream = fs.createReadStream(filePath);
            readStream.pipe(res);

            break;

        case "/wasans":
            let filePath2 = path.join(__dirname, "images", "sans.jpg");
            let fileStat2 = fs.statSync(filePath2);
            res.writeHead(200, {"Content-Type":"image/jpg", "Content-Length":fileStat2.size});
            
            let readStream2 = fs.createReadStream(filePath2);
            readStream2.pipe(res);
            break;
    }




});

server.listen(32000, () =>{
    console.log("Server is running on 32000");
});