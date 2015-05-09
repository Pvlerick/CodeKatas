module StringCalculator

open System
open Xunit

let add numbers =
    match numbers with
    | "" -> 0
    | _ ->
        let parse (content: string) (delim: char) =
            content.Split('\n')
            |> Seq.collect (fun l -> l.Split(delim))
            |> Seq.sumBy Int32.Parse
        if numbers.StartsWith("//") then
            parse (numbers.Substring 4) numbers.[2]
        else parse numbers ','


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
[<InlineData("42,-5", 37)>]
let Add_With_TwoNumbers_Must_ReturnSumOfNumbersAsInt(value: string, expected: int) =
    let result = add value
    Assert.Equal(expected, result)

[<Theory>]
[<InlineData("0,1,2", 3)>]
[<InlineData("1,5,8,9", 23)>]
[<InlineData("0,1,2,3,5,8,13,21", 53)>]
[<InlineData("0,10,0,5,-8", 7)>]
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