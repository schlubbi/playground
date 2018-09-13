fn string_literal() {
    let a = "some string";
}

fn string() {
    // immutable by default
    let a = String::from("some string");

    let mut mut_a = String::from("some mutable string");

    let a = String::from("ASDASD");

    mut_a.push_str(", yo!");
}

fn move_reference() {
    let s = String::from("Hello");
    let s2 = s;
}

fn clone() {
    let s = String::from("hello");
    let s2 = s.clone();
}
