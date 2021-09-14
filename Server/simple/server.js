let result = {
    name:"경기게임마이스터고등학교",
    openYear: 2020,
    circle:[
        {id:1, name:"GMGM"},
        {id:2, name:"게임기획"},
        {id:3, name:"픽셀웨일"}
    ],
    grade:{
        1:[
            {name:"최희진", grade:"C"},
            {name:"서동연", grade:"B"},
            {name:"이태영", grade:"C"}
        ],
        2:[
            {name:"유지호", grade:"A"},
            {name:"임동하", grade:"C"}
        ],
        3:[
            {name:"서선호", grade:"A"},
            {name:"강기호", grade:"B"},
        ],
    },
    "teacherList":[
        "최선한", "김은정","이하은"
    ]
};

function add(circle, name, grade){
    let c = result.circle.find(x => x.name == circle);
    if(c == undefined) {
        console.log("해당 동아리는 존재하지 않습니다.");
    }else{
        result.grade[c.id].push({name:name, grade:grade});
    }
}

// let a = [1,2,3,4,5];
// a.push(10);
// => [1,2,3,4,5,10]

add("GMGM2", "한우엽", "A");
add("픽셀웨일", "김경혁", "B");
add("게임기획", "김태현", "A");

let str = JSON.stringify(result);
console.log(str);



