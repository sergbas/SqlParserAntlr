using Antlr4.Runtime.Misc;
using SqlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlParserAntlr
{
    class Program
    {
        static void Main(string[] args)
        {
            var TestPrinter1 = new Printer();
            Parser.Parse("SELECT * FROM Somewhere", TestPrinter1, SQLType.TSql);
            Console.WriteLine(TestPrinter1.StatementFound);
            TestPrinter1 = new Printer();
            Parser.Parse("ALTER TABLE [dbo].[SelectOption_] ADD FOREIGN KEY ([User_Creator_ID_]) REFERENCES [dbo].[User_]([ID_])", TestPrinter1, SQLType.TSql);
            Console.WriteLine(TestPrinter1.StatementFound);

            var TestPrinter2 = new WherePrinter();
            Parser.Parse(@"SELECT [ID_],[UserName_],[StartDate_],[EndDate_],[EmployeeNumber_]
      ,[OrientationDate_],[FirstName_],[LastName_],[Title_],[MiddleName_]
      ,[NickName_],[Prefix_],[Suffix_],[Active_]
FROM [User_]
where [UserName_]=@UserName", TestPrinter2, SQLType.TSql);

            Console.WriteLine(TestPrinter2.SearchList);
            Console.WriteLine("Username:" + TestPrinter2.SearchList[0]);


            var TestPrinter3 = new WherePrinter();
            Parser.Parse(@"SELECT [ID_],[UserName_],[StartDate_],[EndDate_],[EmployeeNumber_]
      ,[OrientationDate_],[FirstName_],[LastName_],[Title_],[MiddleName_]
      ,[NickName_],[Prefix_],[Suffix_],[Active_]
FROM [User_]
where [UserName_] LIKE @UserName+'%'", TestPrinter3, SQLType.TSql);
            Console.WriteLine(TestPrinter3.SearchList);
            Console.WriteLine("UserName:{0}", TestPrinter3.SearchList[0]);


            Console.ReadLine();
        }
    }

    public class Printer : TSqlParserBaseListener
    {
        public bool StatementFound { get; set; }

        public override void EnterDml_clause([NotNull] TSqlParser.Dml_clauseContext context)
        {
            StatementFound |= context.select_statement() != null;
            base.EnterDml_clause(context);
        }
    }

    public class WherePrinter : TSqlParserBaseListener
    {
        public WherePrinter()
        {
            SearchList = new List<string>();
        }

        public List<string> SearchList { get; set; }

        public override void EnterExpression([NotNull] TSqlParser.ExpressionContext context)
        {
            var LocalID = context?.primitive_expression()?.LOCAL_ID()?.GetText();
            if (!string.IsNullOrEmpty(LocalID))
                SearchList.Add(LocalID.Replace("@", ""));
            base.EnterExpression(context);
        }
    }
}
