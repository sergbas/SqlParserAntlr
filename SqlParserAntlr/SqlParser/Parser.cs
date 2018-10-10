using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SqlParserAntlr;
using System.IO;

namespace SqlParser
{
    public static class Parser
    {
        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="listener">The listener.</param>
        /// <param name="sqlType">Type of the SQL.</param>
        public static void Parse(string input, IParseTreeListener listener, SQLType sqlType)
        {
            if (sqlType == SQLType.TSql)
                ParseTSQL(input, listener);
        }

        /// <summary>
        /// Parses the TSQL.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="listener">The listener.</param>
        private static void ParseTSQL(string input, IParseTreeListener listener)
        {
            ICharStream Stream = CharStreams.fromstring(input);
            Stream = new CaseChangingCharStream(Stream);
            ITokenSource Lexer = new TSqlLexer(Stream, TextWriter.Null, TextWriter.Null);
            ITokenStream Tokens = new CommonTokenStream(Lexer);
            TSqlParser Parser = new TSqlParser(Tokens, TextWriter.Null, TextWriter.Null)
            {
                BuildParseTree = true
            };
            IParseTree tree = Parser.tsql_file();
            ParseTreeWalker.Default.Walk(listener, tree);
        }
    }
}