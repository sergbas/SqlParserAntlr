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

        private enum JoinMode
        {
            Undefined,
            Where,
            Join
        };
        private JoinMode mode;
        private enum BranchType
        {
            Select,
            Table_sources,
            Search_condition
            //Join
        };
        private BranchType branch;

        private string alias = "";

        public override void EnterQuery_specification(TSqlParser.Query_specificationContext ctx)
        {
            mode = JoinMode.Undefined;
        }
        public override void EnterTable_sources(TSqlParser.Table_sourcesContext ctx)
        {
            if (ctx.ChildCount > 1)
                mode = JoinMode.Where;
            branch = BranchType.Table_sources;
        }
        public override void EnterTable_source_item_joined([NotNull] TSqlParser.Table_source_item_joinedContext ctx)
        {
            if ((mode == JoinMode.Undefined & ctx.ChildCount == 1) || (mode == JoinMode.Where))
                return;
            mode = JoinMode.Join;
            branch = BranchType.Table_sources;
        }
        public override void EnterTable_name_with_hint([NotNull] TSqlParser.Table_name_with_hintContext ctx)
        {
            if (mode == JoinMode.Undefined)
                return;
            if (branch == BranchType.Table_sources)
                Console.WriteLine(branch.ToString());
            alias = "";
        }
        public override void EnterTable_name([NotNull] TSqlParser.Table_nameContext ctx)
        {
            if (branch == BranchType.Search_condition || branch == BranchType.Select || mode == JoinMode.Undefined)
                return;
            Console.WriteLine(ctx.GetText());
        }
        public override void EnterTable_alias([NotNull] TSqlParser.Table_aliasContext ctx)
        {
            if (branch == BranchType.Search_condition || branch == BranchType.Select | mode == JoinMode.Undefined)
                return;
            alias = ctx.GetChild(0).GetText();
            Console.WriteLine("alias=" + alias);
        }
        public override void EnterSearch_condition([NotNull] TSqlParser.Search_conditionContext ctx)
        {
            if (mode == JoinMode.Undefined)
                return;
            branch = BranchType.Search_condition;
            Console.WriteLine("Search_condition");
            Console.WriteLine(ctx.GetText());
            return;
        }
        public override void EnterSelect_statement([NotNull] TSqlParser.Select_statementContext ctx)
        {
            Console.WriteLine("Select_statement");
            branch = BranchType.Select;
            return;
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
