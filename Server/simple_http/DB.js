const { connectionData } = require("./secret.js");
const mysql = require("mysql2");

// 접속정보를 기반으로 연결 풀을 만든다.
const pool = mysql.createPool(connectionData);

// 프로미스 기반의 풀을 만든다.
const promisePool = pool.promise();

async function insertData(name, msg, score) {
    try
    {

        let sql = `INSERT INTO high_scores (name, msg, score)
        VALUES (?, ?, ?)`;
        
        let result = await promisePool.query(sql, [name, msg, score]);
        
        return result[0].affectedRows == 1;
    }
    catch
    {
        return false;
    }
    
}

module.exports = {
    pool: promisePool,
    insertData
};