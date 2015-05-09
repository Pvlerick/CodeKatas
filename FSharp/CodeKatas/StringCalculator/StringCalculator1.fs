module StringCalculator

open System
open Xunit

let add numbers =
    match numbers with
    | "" -> 0
    | _ ->
        let parse (content: string) delimiters =
            content.Split(delimiters)
            |> Seq.map Int32.Parse
            |> Seq.filter (fun i -> i <= 1000)

        let ns =
            if numbers.StartsWith("//")
                then parse (numbers.Substring 4) [| '\n'; numbers.[2] |]
                else parse numbers [| '\n'; ',' |]

        if ns |> Seq.exists (fun i -> i < 0)
            then failwith ("negatives not allowed: " + String.Join(",", (ns |> Seq.filter (fun i -> i < 0) |> Seq.toArray)))
            else ns |> Seq.sum

[<Fact>]
let Add_With_EmptyString_Must_ReturnZero() =
    let result = add ""
    Assert.Equal(0, result)

[<Theory>]
[<InlineData("0", 0)>]
[<InlineData("1", 1)>]
[<InlineData("2", 2)>]
[<InlineData("42", 42)>]
let Add_With_OneNumber_Must_ReturnNumberAsInt(value: string, expected: int) =
    let result = add value
    Assert.Equal(expected, result)

[<Theory>]
[<InlineData("0,1", 1)>]
[<InlineData("1,5", 6)>]
[<InlineData("42,0", 42)>]
[<InlineData("0,42", 42)>]
[<InlineData("42,5", 47)>]
let Add_With_TwoNumbers_Must_ReturnSumOfNumbersAsInt(value: string, expected: int) =
    let result = add value
    Assert.Equal(expected, result)

[<Theory>]
[<InlineData("0,1,2", 3)>]
[<InlineData("1,5,8,9", 23)>]
[<InlineData("0,1,2,3,5,8,13,21", 53)>]
[<InlineData("0,10,0,5,8", 23)>]
let Add_With_UnknownAmountOfNumbers_Must_ReturnSumOfNumbersAsInt(value: string, expected: int) =
    let result = add value
    Assert.Equal(expected, result)

[<Theory>]
[<InlineData("1\n2,3", 6)>]
[<InlineData("1,5\n8,9", 23)>]
let Add_With_UnknownAmountOfNumbersWithLineBreaks_Must_ReturnSumOfNumbersAsInt(value: string, expected: int) =
    let result = add value
    Assert.Equal(expected, result)

[<Theory>]
[<InlineData("//$\n1$2$3", 6)>]
[<InlineData("//*\n1*9*5\n6", 21)>]
let Add_With_UnknownAmountOfNumbersWithDifferentDelimiter_Must_ReturnSumOfNumbersAsInt(value: string, expected: int) =
    let result = add value
    Assert.Equal(expected, result)

[<Theory>]
[<InlineData("-3,5", "-3")>]
[<InlineData("-2,3,-9", "-2,-9")>]
[<InlineData("//*\n-7*3\n-2", "-7,-2")>]
let Add_With_UnknownAmountOfNumbersWithNegatives_Must_ThrowWithNegativesInTheMessage(value: string, expected: string) =
    let e = Assert.Throws<Exception>(fun () -> add value |> ignore)
    Assert.Equal("negatives not allowed: " + expected, e.Message)


[<Theory>]
[<InlineData("1,5,1000", 1006)>]
[<InlineData("9,3,1001", 12)>]
[<InlineData("//$\n9$3\n1001$1000$1520", 1012)>]
let Add_With_UnknownAmountOfNumbersWithHigherThan1000_Must_IgnoreValuesHigherThan1000(value: string, expected: int) =
    let result = add value
    Assert.Equal(expected, result)