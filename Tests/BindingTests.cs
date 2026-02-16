using Louis.CustomPackages.CommandLineInterface;
using NUnit.Framework;

public class BindingTests {
    [Test]
    public void BindingTest1() {
        CommandSchema schema = new CommandSchema()
            .Required<int[]>("param", 0);
        string cmd = "test [1,2,3]";

        // Parsing Process
        var tokens= CommandTokenizer.Tokenize(cmd);
        var parameters = tokens[1..];
        var parsedTokens = new ParsedTokens(parameters, schema.Shorthands);
        var boundArgs = CommandBinder.Bind(schema, parsedTokens);

        Assert.IsInstanceOf<int[]>(boundArgs.Get<int[]>("param"));
        Assert.IsNotEmpty(boundArgs.Get<int[]>("param"));
        Assert.AreEqual(3, boundArgs.Get<int[]>("param").Length);
        Assert.AreEqual(1, boundArgs.Get<int[]>("param")[0]);
        Assert.AreEqual(2, boundArgs.Get<int[]>("param")[1]);
        Assert.AreEqual(3, boundArgs.Get<int[]>("param")[2]);
    }

    [Test]
    public void BindingTest2() {
        CommandSchema schema = new CommandSchema()
            .Required<TestStruct>("person", 0);

        string cmd = "test person={\"firstname\":\"Bob\",\"surname\":\"Smith\"}";

        // Parsing Process
        var tokens = CommandTokenizer.Tokenize(cmd);
        var parameters = tokens[1..];
        var parsedTokens = new ParsedTokens(parameters, schema.Shorthands);
        var boundArgs = CommandBinder.Bind(schema, parsedTokens);


        object person = boundArgs.Get<TestStruct>("person");
        Assert.IsInstanceOf<TestStruct>(person);
        Assert.AreEqual("Bob", ((TestStruct)person).firstname);
        Assert.AreEqual("Smith", ((TestStruct)person).surname);
    }

    [Test]
    public void BindingTest3() {
        CommandSchema schema = new CommandSchema()
            .Required<TestStruct[]>("people", 0);

        string cmd = "test people=[{\"firstname\":\"Bob\",\"surname\":\"Smith\"},{\"firstname\":\"Jane\",\"surname\":\"Doe\"}]";
        var tokens = CommandTokenizer.Tokenize(cmd);
        var parameters = tokens[1..];
        var parsedTokens = new ParsedTokens(parameters, schema.Shorthands);
        var boundArgs = CommandBinder.Bind(schema, parsedTokens);


        object people = boundArgs.Get<TestStruct[]>("people");
        Assert.IsInstanceOf<TestStruct[]>(people);
        TestStruct[] converted = (TestStruct[]) people;

        Assert.AreEqual("Bob", converted[0].firstname);
        Assert.AreEqual("Smith", converted[0].surname);
        Assert.AreEqual("Jane", converted[1].firstname);
        Assert.AreEqual("Doe", converted[1].surname);
    }

    struct TestStruct {
        public string firstname;
        public string surname;
    }
}
