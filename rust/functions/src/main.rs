fn main() {
    another_function(50);
    yet_another_function(20, 'z');
}

fn another_function(x: i32) {
    println!("The value of x is: {}", x);
}

fn yet_another_function(x: i32, y: char) {
    println!("The char of u is {}", y);
}

fn assignment_statement() {
    let x = 10;
}

fn new_scope() {
    let statement = {
        let y = 4;
        y + 4;
    };

    let expression = {
        let y = 4;
        y + 4
    };

}

fn five() -> i32 {
    5
}

fn plus_one(x: i32) -> i32 {
    x + 1
}

fn nothing() -> () {
    3;
}

fn comments() {
    // above 
    // multi line
    let x = 5; // inline
}
