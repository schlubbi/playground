fn if_expressions() {
    let number = 3;

    if number < 5 {
        println!("YOYOYOYO!");
    } else {
        println!("NONONONO!");
    }

    let still_there: bool = false;

    if still_there {
    }

    if number % 4 == 0 {
    } else if number % 3 == 0 {
    } else if number % 2 == 0 {
    } else {
    }

    let number: i32 = if still_there {
        5
    } else {
        6 + 6
    };
}
