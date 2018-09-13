fn loop() {
    loop {
        println!("Hi");
    }
}

fn while() {
    let number = 3;
    while number != 0 {
        println!("Number: {}", number);

        number--;
    }
}

fn for() {
    let a = [10, 20, 30, 40, 50];

    for element in a.iter() {
        println!("element: {}", element);
    }
}

fn range() {
    for number in (1..4).rev() {
        println!("Reverse: {}", number);
    }
}
