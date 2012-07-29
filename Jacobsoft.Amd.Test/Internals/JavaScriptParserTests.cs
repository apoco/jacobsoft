using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using Jacobsoft.Amd.Internals.AntlrGenerated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jacobsoft.Amd.Test.Internals
{
    [TestClass]
    public class JavaScriptParserTests
    {
        [TestMethod]
        public void ParseIdentifier()
        {
            var program = this.ParseProgram("foo = 3;");
            
            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.Is<NumericLiteral>("3");
        }

        [TestMethod]
        public void ParseNullLiteral()
        {
            var program = this.ParseProgram("foo = null;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.Is<NullLiteral>();
        }

        [TestMethod]
        public void ParseFalseLiteral()
        {
            var program = this.ParseProgram("foo = false;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.Is<FalseLiteral>();
        }

        [TestMethod]
        public void ParseTrueLiteral()
        {
            var program = this.ParseProgram("foo = true;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.Is<TrueLiteral>();
        }

        [TestMethod]
        public void ParseStringLiteral_WithUnicodeCharacters()
        {
            var program = this.ParseProgram(@"foo = '\u0052';");
            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.Is<StringLiteral>(@"'\u0052'");
        }

        [TestMethod]
        public void ParseEmptyObjectLiteral()
        {
            var program = this.ParseProgram("foo = {};");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.Is<ObjectLiteral>();
        }

        [TestMethod]
        public void ParseObjectLiteral()
        {
            var program = this.ParseProgram("foo = {0: null, bar: 1, 'baz': 2};");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var obj = assignment.Value.As<ObjectLiteral>();

            obj.Assignments[0].Property.Is<NumericLiteral>("0");
            obj.Assignments[0].Value.Is<NullLiteral>();

            obj.Assignments[1].Property.Is<Identifier>("bar");
            obj.Assignments[1].Value.Is<NumericLiteral>("1");

            obj.Assignments[2].Property.Is<StringLiteral>("'baz'");
            obj.Assignments[2].Value.Is<NumericLiteral>("2");
        }

        [TestMethod]
        public void ParseThisExpression()
        {
            var program = this.ParseProgram("this.alert('foo');");
            var call = program.Statements[0].As<CallExpression>();
            call.Arguments[0].Is<StringLiteral>("'foo'");

            var propExpr = call.Function.As<PropertyExpression>();
            propExpr.Object.Is<ThisExpression>();
            propExpr.Property.Is<Identifier>("alert");
        }

        [TestMethod]
        public void ParseBitwiseNotExpression()
        {
            var program = this.ParseProgram("foo = ~foo;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var bitwiseNot = assignment.Value.As<BitwiseNotExpression>();
            bitwiseNot.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParseBooleanNotExpression()
        {
            var program = this.ParseProgram("foo = !foo;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var bitwiseNot = assignment.Value.As<BooleanNotExpression>();
            bitwiseNot.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParseNegativeExpression()
        {
            var program = this.ParseProgram("foo = -foo;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var bitwiseNot = assignment.Value.As<NegationExpression>();
            bitwiseNot.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParsePlusExpression()
        {
            var program = this.ParseProgram("foo = +bar;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var bitwiseNot = assignment.Value.As<UnaryPlusExpression>();
            bitwiseNot.Operand.Is<Identifier>("bar");
        }

        [TestMethod]
        public void ParseTypeofExpression()
        {
            var program = this.ParseProgram("foo = typeof(bar);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var typeofExpr = assignment.Value.As<TypeofExpression>();
            typeofExpr.Operand.Is<Identifier>("bar");
        }

        [TestMethod]
        public void ParseVoidExpression()
        {
            var program = this.ParseProgram("foo = void(bar);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var voidExpr = assignment.Value.As<VoidExpression>();
            voidExpr.Operand.Is<Identifier>("bar");
        }

        [TestMethod]
        public void ParseAdditionExpression()
        {
            var program = this.ParseProgram("foo = 3 + 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            
            var addition = assignment.Value.As<AdditionExpression>();
            addition.Operands[0].As<NumericLiteral>();
            addition.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseSubtractionExpression()
        {
            var program = this.ParseProgram("foo = 3 - 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var addition = assignment.Value.As<SubtractionExpression>();
            addition.Operands[0].As<NumericLiteral>();
            addition.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseMultiplyExpression()
        {
            var program = this.ParseProgram("foo = 3 * 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var division = assignment.Value.As<MultiplicationExpression>();
            division.Operands[0].As<NumericLiteral>();
            division.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseDivisionExpression()
        {
            var program = this.ParseProgram("foo = 3 / 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var division = assignment.Value.As<DivisionExpression>();
            division.Operands[0].As<NumericLiteral>();
            division.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseModulusExpression()
        {
            var program = this.ParseProgram("foo = 3 % 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var division = assignment.Value.As<ModulusExpression>();
            division.Operands[0].As<NumericLiteral>();
            division.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseBitwiseAndExpression()
        {
            var program = this.ParseProgram("foo = 3 & 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<BitwiseAndExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseBitwiseOrExpression()
        {
            var program = this.ParseProgram("foo = 3 | 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<BitwiseOrExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseBitwiseXorExpression()
        {
            var program = this.ParseProgram("foo = 3 ^ 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<BitwiseXorExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseAndExpression()
        {
            var program = this.ParseProgram("foo = 3 && 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<LogicalAndExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseOrExpression()
        {
            var program = this.ParseProgram("foo = 3 || 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<LogicalOrExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseShiftLeftExpression()
        {
            var program = this.ParseProgram("foo = 3 << 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<ShiftLeftExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseShiftRightExpression()
        {
            var program = this.ParseProgram("foo = 3 >> 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<ShiftRightExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseRotateRightExpression()
        {
            var program = this.ParseProgram("foo = 3 >>> 4;");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var andExpr = assignment.Value.As<RotateRightExpression>();
            andExpr.Operands[0].As<NumericLiteral>();
            andExpr.Operands[1].As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar == baz);");
            
            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var comp = assignment.Value.As<EqualToExpression>();
            comp.Operands[0].Is<Identifier>("bar");
            comp.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseNotEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar != baz);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var comp = assignment.Value.As<NotEqualToExpression>();
            comp.Operands[0].Is<Identifier>("bar");
            comp.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseStrictEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar === baz);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var comp = assignment.Value.As<StrictlyEqualToExpression>();
            comp.Operands[0].Is<Identifier>("bar");
            comp.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseNotStrictEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar !== baz);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var comp = assignment.Value.As<NotStrictlyEqualToExpression>();
            comp.Operands[0].Is<Identifier>("bar");
            comp.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseGreaterThanExpression()
        {
            var program = this.ParseProgram("foo = (bar > baz);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var comp = assignment.Value.As<GreaterThanExpression>();
            comp.Operands[0].Is<Identifier>("bar");
            comp.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseGreaterThanOrEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar >= baz);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var comp = assignment.Value.As<GreaterThanOrEqualToExpression>();
            comp.Operands[0].Is<Identifier>("bar");
            comp.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseInExpression()
        {
            var program = this.ParseProgram("foo = (bar in baz);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var inExpr = assignment.Value.As<InExpression>();
            inExpr.Operands[0].Is<Identifier>("bar");
            inExpr.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseInstanceOfExpression()
        {
            var program = this.ParseProgram("foo = (bar instanceof baz);");

            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var inExpr = assignment.Value.As<InstanceOfExpression>();
            inExpr.Operands[0].Is<Identifier>("bar");
            inExpr.Operands[1].Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseConditionalExpression()
        {
            var program = this.ParseProgram("foo = bar ? bar : baz;");
            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var condExpr = assignment.Value.As<ConditionalExpression>();
            condExpr.Condition.Is<Identifier>("bar");
            condExpr.TrueExpression.Is<Identifier>("bar");
            condExpr.FalseExpression.Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParsePrefixIncrementer()
        {
            var program = this.ParseProgram("++foo;");
            var incrementer = program.Statements[0].As<PrefixIncrementExpression>();
            incrementer.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParsePostfixIncrementer()
        {
            var program = this.ParseProgram("foo++;");
            var incrementer = program.Statements[0].As<PostfixIncrementExpression>();
            incrementer.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParsePrefixDecrementer()
        {
            var program = this.ParseProgram("--foo;");
            var incrementer = program.Statements[0].As<PrefixDecrementExpression>();
            incrementer.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParsePostfixDecrementer()
        {
            var program = this.ParseProgram("foo--;");
            var incrementer = program.Statements[0].As<PostfixDecrementExpression>();
            incrementer.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParsePlusEqualsExpression()
        {
            var program = this.ParseProgram("foo += 3;");

            var assignment = program.Statements[0].As<AddAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseMinusEqualsExpression()
        {
            var program = this.ParseProgram("foo -= 3;");

            var assignment = program.Statements[0].As<SubtractAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseTimesEqualsExpression()
        {
            var program = this.ParseProgram("foo *= 3;");

            var assignment = program.Statements[0].As<MultiplyAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseDivideEqualsExpression()
        {
            var program = this.ParseProgram("foo /= 3;");

            var assignment = program.Statements[0].As<DivideAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseModEqualsExpression()
        {
            var program = this.ParseProgram("foo %= 3;");

            var assignment = program.Statements[0].As<ModulusAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseBitwiseAndAssignExpression()
        {
            var program = this.ParseProgram("foo &= 3;");

            var assignment = program.Statements[0].As<BitwiseAndAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseBitwiseOrAssignExpression()
        {
            var program = this.ParseProgram("foo |= 3;");

            var assignment = program.Statements[0].As<BitwiseOrAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseBitwiseXorAssignExpression()
        {
            var program = this.ParseProgram("foo ^= 3;");

            var assignment = program.Statements[0].As<BitwiseXorAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseShiftLeftEqualsExpression()
        {
            var program = this.ParseProgram("foo <<= 3;");

            var assignment = program.Statements[0].As<ShiftLeftAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseShiftRightEqualsExpression()
        {
            var program = this.ParseProgram("foo >>= 3;");

            var assignment = program.Statements[0].As<ShiftRightAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseRotateRightEqualsExpression()
        {
            var program = this.ParseProgram("foo >>>= 3;");

            var assignment = program.Statements[0].As<RotateRightAndAssignExpression>();
            assignment.Target.Is<Identifier>("foo");
            assignment.Value.As<NumericLiteral>();
        }

        [TestMethod]
        public void ParseIndexExpression()
        {
            var program = this.ParseProgram("foo = bar[0];");
            var assignment = program.Statements[0].As<AssignmentExpression>();
            assignment.Target.Is<Identifier>("foo");

            var indexExpr = assignment.Value.As<IndexExpression>();
            indexExpr.Object.Is<Identifier>("bar");
            indexExpr.Index.Is<NumericLiteral>("0");
        }

        [TestMethod]
        public void ParsePropertyExpression()
        {
            var program = this.ParseProgram("foo.bar();");
            var call = program.Statements[0].As<CallExpression>();
            var propExpr = call.Function.As<PropertyExpression>();
            propExpr.Object.Is<Identifier>("foo");
            propExpr.Property.Is<Identifier>("bar");
        }

        [TestMethod]
        public void ParseNewExpression()
        {
            var program = this.ParseProgram("foo = new bar(3);");
            var assignment = program.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("foo");

            var newExpr = assignment.Value.As<NewExpression>();
            newExpr.Constructor.Is<Identifier>("bar");
            newExpr.Arguments[0].Is<NumericLiteral>("3");
        }

        [TestMethod]
        public void ParseDeleteExpression()
        {
            var program = this.ParseProgram("delete foo;");
            var deletion = program.Statements[0].As<DeleteExpression>();
            deletion.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParseEmptyStatement()
        {
            var program = this.ParseProgram(";");
            var stmt = program.Statements[0].As<EmptyStatement>();
        }

        [TestMethod]
        public void ParseStatementBlock()
        {
            var program = this.ParseProgram("{ bar = baz; }");
            var block = program.Statements[0].As<StatementBlock>();
            var assignment = block.Statements[0].As<AssignExpression>();
            assignment.Target.Is<Identifier>("bar");
            assignment.Value.Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseVariableStatement()
        {
            var program = this.ParseProgram("var foo;");
            var stmt = program.Statements[0].As<VariableStatement>();
            var decl = stmt.Declarations[0].As<VariableDeclaration>();
            decl.Identifier.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParseVariableStatementWithInitializer()
        {
            var program = this.ParseProgram("var foo = 3;");
            var stmt = program.Statements[0].As<VariableStatement>();
            var decl = stmt.Declarations[0].As<VariableDeclaration>();
            decl.Identifier.Is<Identifier>("foo");
            decl.InitialValue.Is<NumericLiteral>("3");
        }

        [TestMethod]
        public void ParseIfStatement()
        {
            var program = this.ParseProgram("if (foo) { bar(3); }");
            var ifStmt = program.Statements[0].As<IfStatement>();
            ifStmt.Condition.Is<Identifier>("foo");
            var block = ifStmt.Statement.As<StatementBlock>();
            var call = block.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("bar");
            call.Arguments[0].Is<NumericLiteral>("3");
            Assert.IsNull(ifStmt.ElseStatement);
        }

        [TestMethod]
        public void ParseIfElseStatement()
        {
            var program = this.ParseProgram("if (foo) bar(3); else alert('baz');");
            var ifStmt = program.Statements[0].As<IfStatement>();
            ifStmt.Condition.Is<Identifier>("foo");
            
            var call = ifStmt.Statement.As<CallExpression>();
            call.Function.Is<Identifier>("bar");
            call.Arguments[0].Is<NumericLiteral>("3");

            call = ifStmt.ElseStatement.As<CallExpression>();
            call.Function.Is<Identifier>("alert");
            call.Arguments[0].Is<StringLiteral>("'baz'");
        }

        [TestMethod]
        public void ParseSwitchStatement()
        {
            var program = this.ParseProgram("switch (foo) { case 1: bar(3); break; case 2: baz(3); break; default: alert('foo!'); }");
            var switchStmt = program.Statements[0].As<SwitchStatement>();
            
            switchStmt.Expression.Is<Identifier>("foo");
            switchStmt.CaseClauses[0].Expression.Is<NumericLiteral>("1");
            switchStmt.CaseClauses[1].Expression.Is<NumericLiteral>("2");

            var call = switchStmt.CaseClauses[0].Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("bar");
            call.Arguments[0].Is<NumericLiteral>("3");
            switchStmt.CaseClauses[0].Statements[1].As<BreakStatement>();

            call = switchStmt.CaseClauses[1].Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("baz");
            call.Arguments[0].Is<NumericLiteral>("3");
            switchStmt.CaseClauses[1].Statements[1].As<BreakStatement>();

            call = switchStmt.DefaultClause.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("alert");
            call.Arguments[0].Is<StringLiteral>("'foo!'");
        }

        [TestMethod]
        public void ParseForLoop()
        {
            var program = this.ParseProgram("for (var i = 0; i < foo; i++) bar(i);");
            var forStmt = program.Statements[0].As<ForStatement>();

            var varStmt = forStmt.InitializationStatement.As<VariableStatement>();
            varStmt.Declarations[0].Identifier.Is<Identifier>("i");
            varStmt.Declarations[0].InitialValue.Is<NumericLiteral>("0");

            var comp = forStmt.Condition.As<LessThanExpression>();
            comp.Operands[0].Is<Identifier>("i");
            comp.Operands[1].Is<Identifier>("foo");

            var incr = forStmt.IncrementStatement.As<PostfixIncrementExpression>();
            incr.Operand.Is<Identifier>("i");

            var call = forStmt.LoopStatement.As<CallExpression>();
            call.Function.Is<Identifier>("bar");
            call.Arguments[0].Is<Identifier>("i");
        }

        [TestMethod]
        public void ParseForInStatement()
        {
            var program = this.ParseProgram("for (var foo in bar) { foo++; }");
            var stmt = program.Statements[0].As<ForInStatement>();
            
            var decl = stmt.LoopVariable.As<VariableDeclaration>();
            decl.Identifier.Is<Identifier>("foo");
            stmt.Object.Is<Identifier>("bar");

            var block = stmt.LoopStatement.As<StatementBlock>();
            var incr = block.Statements[0].As<PostfixIncrementExpression>();
            incr.Operand.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParseWhileLoop()
        {
            var program = this.ParseProgram("while (true) { foo(); }");
            var loop = program.Statements[0].As<WhileStatement>();
            loop.Condition.As<TrueLiteral>();

            var block = loop.Statement.As<StatementBlock>();
            var call = block.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("foo");
        }

        [TestMethod]
        public void ParseDoWhileLoop()
        {
            var program = this.ParseProgram("do { foo.bar(); } while(foo.baz);");
            var doStmt = program.Statements[0].As<DoWhileStatement>();

            var block = doStmt.Statement.As<StatementBlock>();
            var callExpr = block.Statements[0].As<CallExpression>();
            var propExpr = callExpr.Function.As<PropertyExpression>();
            propExpr.Object.Is<Identifier>("foo");
            propExpr.Property.Is<Identifier>("bar");

            propExpr = doStmt.Condition.As<PropertyExpression>();
            propExpr.Object.Is<Identifier>("foo");
            propExpr.Property.Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseContinueStatement()
        {
            var program = this.ParseProgram("for (var i = 1; i <= numFloors; i++) { if (i == 13) continue; alert(i); }");
            var forLoop = program.Statements[0].As<ForStatement>();
            var block = forLoop.LoopStatement.As<StatementBlock>();
            var ifStmt = block.Statements[0].As<IfStatement>();
            ifStmt.Statement.As<ContinueStatement>();
        }

        [TestMethod]
        public void ParseThrowStatement()
        {
            var program = this.ParseProgram("throw new NotImplementedException();");
            var throwStmt = program.Statements[0].As<ThrowStatement>();
            var newExpr = throwStmt.Expression.As<NewExpression>();
            newExpr.Constructor.Is<Identifier>("NotImplementedException");
        }

        [TestMethod]
        public void ParseLabeledStatement()
        {
            var program = this.ParseProgram(@"
                outer: 
                for (foo in bar) { 
                    for (baz in bar[foo]) { 
                        if (baz == 'name') { 
                            name = bar[foo][baz]; 
                            break outer; 
                        } 
                    } 
                }");

            var labeledStmt = program.Statements[0].As<LabeledStatement>();
            labeledStmt.Label.Is<Identifier>("outer");

            var loop = labeledStmt.Statement.As<ForInStatement>();
            loop.LoopVariable.Is<Identifier>("foo");
            loop.Object.Is<Identifier>("bar");

            var block = loop.LoopStatement.As<StatementBlock>();
            loop = block.Statements[0].As<ForInStatement>();
            loop.LoopVariable.Is<Identifier>("baz");
            var indexExpr = loop.Object.As<IndexExpression>();
            indexExpr.Object.Is<Identifier>("bar");
            indexExpr.Index.Is<Identifier>("foo");

            block = loop.LoopStatement.As<StatementBlock>();
            var ifStmt = block.Statements[0].As<IfStatement>();
            var equalsExpr = ifStmt.Condition.As<EqualToExpression>();
            equalsExpr.Operands[0].Is<Identifier>("baz");
            equalsExpr.Operands[1].Is<StringLiteral>("'name'");

            block = ifStmt.Statement.As<StatementBlock>();
            var assignExpr = block.Statements[0].As<AssignExpression>();
            assignExpr.Target.Is<Identifier>("name");

            indexExpr = assignExpr.Value.As<IndexExpression>();
            indexExpr.Index.Is<Identifier>("baz");
            indexExpr = indexExpr.Object.As<IndexExpression>();
            indexExpr.Index.Is<Identifier>("foo");
            indexExpr.Object.Is<Identifier>("bar");

            var breakStmt = block.Statements[1].As<BreakStatement>();
            breakStmt.Label.Is<Identifier>("outer");
        }

        [TestMethod]
        public void ParseTryStatement()
        {
            var program = this.ParseProgram("try { foo(); } catch (ex) { bar(ex); } finally { baz(); }");
            var tryStmt = program.Statements[0].As<TryStatement>();

            var call = tryStmt.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("foo");

            Assert.AreEqual("ex", tryStmt.CatchClause.Identifier.Text);
            call = tryStmt.CatchClause.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("bar");
            call.Arguments[0].Is<Identifier>("ex");

            call = tryStmt.FinallyClause.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseWithStatement()
        {
            var program = this.ParseProgram("with (foo) { bar(); baz(); }");
            var withStmt = program.Statements[0].As<WithStatement>();
            var block = withStmt.Statement.As<StatementBlock>();
            withStmt.Expression.Is<Identifier>("foo");
            
            var call = block.Statements[0].As<CallExpression>();
            call.Function.Is<Identifier>("bar");

            call = block.Statements[1].As<CallExpression>();
            call.Function.Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseFunctionExpression()
        {
            var program = this.ParseProgram("foo = function (bar, baz) { bar = baz; };");
            
            var assign = program.Children[0].As<AssignExpression>();
            assign.Target.Is<Identifier>("foo");

            var funcExpr = assign.Value.As<FunctionExpression>();
            funcExpr.Parameters[0].Is<Identifier>("bar");
            funcExpr.Parameters[1].Is<Identifier>("baz");

            assign = funcExpr.Statements[0].As<AssignExpression>();
            assign.Target.Is<Identifier>("bar");
            assign.Value.Is<Identifier>("baz");
        }

        [TestMethod]
        public void ParseEmptyFunctionExpression()
        {
            var program = this.ParseProgram("foo = function () { };");

            var assign = program.Children[0].As<AssignExpression>();
            assign.Target.Is<Identifier>("foo");
            assign.Value.As<FunctionExpression>();
        }

        [TestMethod]
        public void ParseFunctionDeclaration()
        {
            var program = this.ParseProgram("function foo(bar, baz) { bar = baz; }");
            var decl = program.Statements[0].As<FunctionDeclaration>();

            decl.Children[0].Is<Identifier>("foo");

            var paramList = decl.Children[1].As<FormalParameterList>();
            paramList.Children[0].Is<Identifier>("bar");
            paramList.Children[1].Is<Identifier>("baz");

            var assignment = decl.Statements[0].As<AssignExpression>();
            assignment.Children[0].Is<Identifier>("bar");
            assignment.Children[1].Is<Identifier>("baz");
        }

        private Program ParseProgram(string code)
        {
            return JavaScriptTestHelper.ParseProgram(code);
        }
    }
}
