using Louis.CustomPackages.CommandLineInterface.Core;
using NUnit.Framework;

public class BindingTests {
    [Test]
    public void TestingArrayBinding() {
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
    public void TestingJsonBinding() {
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
    public void TestingMultiLineBinding() {
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

    [Test]
    public void TestingEnumBinding() {
        CommandSchema schema = new CommandSchema()
            .Required<TestEnum>("mode", 0);

        string cmd1 = "test mode=AlwaysOpen";
        string cmd2 = "test mode=AlwaysClosed";
        string cmd3 = "test mode=OpenOnMessage";

        var tokens1 = CommandTokenizer.Tokenize(cmd1);
        var tokens2 = CommandTokenizer.Tokenize(cmd2);
        var tokens3 = CommandTokenizer.Tokenize(cmd3);

        var params1 = tokens1[1..];
        var params2 = tokens2[1..];
        var params3 = tokens3[1..];

        var parsed1 = new ParsedTokens(params1, schema.Shorthands);
        var parsed2 = new ParsedTokens(params2, schema.Shorthands);
        var parsed3 = new ParsedTokens(params3, schema.Shorthands);

        var bound1 = CommandBinder.Bind(schema, parsed1);
        var bound2 = CommandBinder.Bind(schema, parsed2);
        var bound3 = CommandBinder.Bind(schema, parsed3);

        Assert.AreEqual(TestEnum.AlwaysOpen, bound1.Get<TestEnum>("mode"));
        Assert.AreEqual(TestEnum.AlwaysClosed, bound2.Get<TestEnum>("mode"));
        Assert.AreEqual(TestEnum.OpenOnMessage, bound3.Get<TestEnum>("mode"));
    }

    [Test]
    public void TestingStringBinding() {
        CommandSchema schema = new CommandSchema()
            .Required<string>("message", 0);
        string cmd1 = "test \"Hello, World!\"";
        string cmd2 = "test message=\"Hello, World!\"";

        var tokens1 = CommandTokenizer.Tokenize(cmd1);
        var tokens2 = CommandTokenizer.Tokenize(cmd2);

        var parameters1 = tokens1[1..];
        var parameters2 = tokens2[1..];

        var parsedTokens1 = new ParsedTokens(parameters1, schema.Shorthands);
        var parsedTokens2 = new ParsedTokens(parameters2, schema.Shorthands);

        var boundArgs1 = CommandBinder.Bind(schema, parsedTokens1);
        var boundArgs2 = CommandBinder.Bind(schema, parsedTokens2);

        Assert.AreEqual("Hello, World!", boundArgs1.Get<string>("message"));
        Assert.AreEqual("Hello, World!", boundArgs2.Get<string>("message"));
    }

    struct TestStruct {
        public string firstname;
        public string surname;
    }

    enum TestEnum {
        AlwaysOpen,
        AlwaysClosed,
        OpenOnMessage
    }
}
