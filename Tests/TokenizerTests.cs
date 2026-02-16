using Louis.CustomPackages.CommandLineInterface;
using NUnit.Framework;

public class TokenizerTests {
    // A Test behaves as an ordinary method
    [Test]
    public void TokenizerTest1() {
        // Use the Assert class to test conditions

        string cmd = @"outputRandom choices=[1, 2, 3]";

        string[] tokens = CommandTokenizer.Tokenize(cmd);

        Assert.AreEqual("outputRandom", tokens[0]);
        Assert.AreEqual("choices=[1, 2, 3]", tokens[1]);
    }

    [Test]
    public void TokenizerTest2() {
        string cmd = @"outputRandom [
              ""foo"",
              ""bar""
            ]";
        string[] tokens = CommandTokenizer.Tokenize(cmd);

        Assert.AreEqual("outputRandom", tokens[0]);
        Assert.AreEqual("[\"foo\", \"bar\"]", tokens[1]);
    }

    [Test]
    public void TokenizerTest3() {
        string cmd = @"exampleFunction data={
              firstname: ""Bob"",   
              surname:    ""Smith""
            }";

        string[] tokens = CommandTokenizer.Tokenize(cmd);

        Assert.AreEqual("exampleFunction", tokens[0]);
        Assert.AreEqual("data={firstname:\"Bob\", surname:\"Smith\"}", tokens[1]);
    }
}
