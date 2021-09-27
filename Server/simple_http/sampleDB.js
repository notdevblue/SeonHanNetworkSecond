const { connectionData } = require("./secret.js");
const mysql = require("mysql2");

// 접속정보를 기반으로 연결 풀을 만든다.
const pool = mysql.createPool(connectionData);

// 프로미스 기반의 풀을 만든다.
const promisePool = pool.promise();

async function insertData(name, msg, score) {
    let sql = `INSERT INTO high_scores (name, msg, score)
               VALUES (?, ?, ?)`;
    
    let result = await promisePool.query(sql, [name, msg, score]);

    console.log(result);
}

async function getList()
{
    let sql = "SELECT name, msg, score FROM high_scores";
    let [rows] = await promisePool.query(sql); // 배열에 첫번째 원소만 가져옴

    console.log(rows);
}

exports.insertData = insertData;
exports.getList = getList;

//insertData("엄준식", "잘놀다갑니다", 154);

//getList();