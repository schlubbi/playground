fn main() {
    let mut x = 5;
    println!("The value of x is: {}", x);
    x = 6;
    println!("The value of x is: {}", x);
    const MAX_PORT: u32 = 100_000;
    println!("MAX_PORT: {}", MAX_PORT);
    println!("Shadowing");
    let a = 5;
    let a = a * 5;
    let a = a + 1;
    println!("a == {}", a);

    let spaces = "    ";
    let spaces = spaces.len();
    println!("# spaces: {}", spaces);
}

fn integer_types() {
    let n: u8  =  8;
    let n: u16 = 16;
    let n: u32 = 32;
    let n: u64 = 64;
    let n: usize = 64;
    let n: i8  =  8;
    let n: i16 = 16;
    let n: i32 = 32;
    let n: i64 = 64;
    let n: isize = 64isize;
}

fn integer_literals() {
    let decimal = 100_1000;
    let hex = 0xff;
    let octal = 0o77;
    let binary = 0b101010101_1010;
    let byte = b'A';
}

fn floating_types() {
    let n: f32 = 0.0;
    let n: f64 = 0.0;
}

fn numeric_operations() {
    let sum = 5 + 5;
    let difference = 5 - 5;
    let product = 5 * 5;
    let quotient = 5.0 / 2.5;
    let remainder = 43 % 17;
}

fn boolean_values() {
    let t = true;
    let f = false;
}

fn character_type() {
    let c = 'z';
    let c: char = 'z';
}

fn compound_types() {
    fn tuples() {
        // tup binds to the entire tuple, considered a single compound element
        let tup:(i32, char, u8, bool) = (32, 'x', 2, true);
        let tup = (32, 'x', 2, true);

        // deconstructing
        let (x, y, z, a) = tup;
        println!("x: {}, y: {}, z: {}, a: {}", x, y, z, a);

        println!("x: {}", tup.0);
        println!("y: {}", tup.1);
        println!("z: {}", tup.2);
        println!("a: {}", tup.3);
    }

    fn arrays() {
        // can not grow/shrink size
        let a = [1, 2, 3, 4, 5];

        let first = a[0];
        let last = a[4];
    }
}
