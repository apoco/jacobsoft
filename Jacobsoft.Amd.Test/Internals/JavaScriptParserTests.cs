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
            
            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>("3", assignment.Value);
        }

        [TestMethod]
        public void ParseNullLiteral()
        {
            var program = this.ParseProgram("foo = null;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NullLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseFalseLiteral()
        {
            var program = this.ParseProgram("foo = false;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<FalseLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseTrueLiteral()
        {
            var program = this.ParseProgram("foo = true;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<TrueLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseEmptyObjectLiteral()
        {
            var program = this.ParseProgram("foo = {};");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<ObjectLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseObjectLiteral()
        {
            var program = this.ParseProgram("foo = {0: null, bar: 1, 'baz': 2};");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            
            var obj = this.AssertNode<ObjectLiteral>(assignment.Value);

            this.AssertNode<NumericLiteral>("0", obj.Assignments[0].Property);
            this.AssertNode<NullLiteral>(obj.Assignments[0].Value);

            this.AssertNode<Identifier>("bar", obj.Assignments[1].Property);
            this.AssertNode<NumericLiteral>("1", obj.Assignments[1].Value);

            this.AssertNode<StringLiteral>("'baz'", obj.Assignments[2].Property);
            this.AssertNode<NumericLiteral>("2", obj.Assignments[2].Value);
        }

        [TestMethod]
        public void ParseThisExpression()
        {
            var program = this.ParseProgram("this.alert('foo');");
            var call = this.AssertNode<CallExpression>(program.Statements[0]);
            this.AssertNode<StringLiteral>("'foo'", call.Arguments[0]);

            var propExpr = this.AssertNode<PropertyExpression>(call.Function);
            this.AssertNode<ThisExpression>(propExpr.Object);
            this.AssertNode<Identifier>("alert", propExpr.Property);
        }

        [TestMethod]
        public void ParseBitwiseNotExpression()
        {
            var program = this.ParseProgram("foo = ~foo;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var bitwiseNot = this.AssertNode<BitwiseNotExpression>(assignment.Value);
            this.AssertNode<Identifier>("foo", bitwiseNot.Operand);
        }

        [TestMethod]
        public void ParseBooleanNotExpression()
        {
            var program = this.ParseProgram("foo = !foo;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var bitwiseNot = this.AssertNode<BooleanNotExpression>(assignment.Value);
            this.AssertNode<Identifier>("foo", bitwiseNot.Operand);
        }

        [TestMethod]
        public void ParseNegativeExpression()
        {
            var program = this.ParseProgram("foo = -foo;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var bitwiseNot = this.AssertNode<NegationExpression>(assignment.Value);
            this.AssertNode<Identifier>("foo", bitwiseNot.Operand);
        }

        [TestMethod]
        public void ParsePlusExpression()
        {
            var program = this.ParseProgram("foo = +bar;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var bitwiseNot = this.AssertNode<UnaryPlusExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", bitwiseNot.Operand);
        }

        [TestMethod]
        public void ParseTypeofExpression()
        {
            var program = this.ParseProgram("foo = typeof(bar);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var typeofExpr = this.AssertNode<TypeofExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", typeofExpr.Operand);
        }

        [TestMethod]
        public void ParseVoidExpression()
        {
            var program = this.ParseProgram("foo = void(bar);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var voidExpr = this.AssertNode<VoidExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", voidExpr.Operand);
        }

        [TestMethod]
        public void ParseAdditionExpression()
        {
            var program = this.ParseProgram("foo = 3 + 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            
            var addition = this.AssertNode<AdditionExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(addition.Operands[0]);
            this.AssertNode<NumericLiteral>(addition.Operands[1]);
        }

        [TestMethod]
        public void ParseSubtractionExpression()
        {
            var program = this.ParseProgram("foo = 3 - 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var addition = this.AssertNode<SubtractionExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(addition.Operands[0]);
            this.AssertNode<NumericLiteral>(addition.Operands[1]);
        }

        [TestMethod]
        public void ParseMultiplyExpression()
        {
            var program = this.ParseProgram("foo = 3 * 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var division = this.AssertNode<MultiplicationExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(division.Operands[0]);
            this.AssertNode<NumericLiteral>(division.Operands[1]);
        }

        [TestMethod]
        public void ParseDivisionExpression()
        {
            var program = this.ParseProgram("foo = 3 / 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var division = this.AssertNode<DivisionExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(division.Operands[0]);
            this.AssertNode<NumericLiteral>(division.Operands[1]);
        }

        [TestMethod]
        public void ParseModulusExpression()
        {
            var program = this.ParseProgram("foo = 3 % 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var division = this.AssertNode<ModulusExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(division.Operands[0]);
            this.AssertNode<NumericLiteral>(division.Operands[1]);
        }

        [TestMethod]
        public void ParseBitwiseAndExpression()
        {
            var program = this.ParseProgram("foo = 3 & 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<BitwiseAndExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseBitwiseOrExpression()
        {
            var program = this.ParseProgram("foo = 3 | 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<BitwiseOrExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseBitwiseXorExpression()
        {
            var program = this.ParseProgram("foo = 3 ^ 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<BitwiseXorExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseAndExpression()
        {
            var program = this.ParseProgram("foo = 3 && 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<LogicalAndExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseOrExpression()
        {
            var program = this.ParseProgram("foo = 3 || 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<LogicalOrExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseShiftLeftExpression()
        {
            var program = this.ParseProgram("foo = 3 << 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<ShiftLeftExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseShiftRightExpression()
        {
            var program = this.ParseProgram("foo = 3 >> 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<ShiftRightExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseRotateRightExpression()
        {
            var program = this.ParseProgram("foo = 3 >>> 4;");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var andExpr = this.AssertNode<RotateRightExpression>(assignment.Value);
            this.AssertNode<NumericLiteral>(andExpr.Operands[0]);
            this.AssertNode<NumericLiteral>(andExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar == baz);");
            
            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var comp = this.AssertNode<EqualToExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", comp.Operands[0]);
            this.AssertNode<Identifier>("baz", comp.Operands[1]);
        }

        [TestMethod]
        public void ParseNotEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar != baz);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var comp = this.AssertNode<NotEqualToExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", comp.Operands[0]);
            this.AssertNode<Identifier>("baz", comp.Operands[1]);
        }

        [TestMethod]
        public void ParseStrictEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar === baz);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var comp = this.AssertNode<StrictlyEqualToExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", comp.Operands[0]);
            this.AssertNode<Identifier>("baz", comp.Operands[1]);
        }

        [TestMethod]
        public void ParseNotStrictEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar !== baz);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var comp = this.AssertNode<NotStrictlyEqualToExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", comp.Operands[0]);
            this.AssertNode<Identifier>("baz", comp.Operands[1]);
        }

        [TestMethod]
        public void ParseGreaterThanExpression()
        {
            var program = this.ParseProgram("foo = (bar > baz);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var comp = this.AssertNode<GreaterThanExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", comp.Operands[0]);
            this.AssertNode<Identifier>("baz", comp.Operands[1]);
        }

        [TestMethod]
        public void ParseGreaterThanOrEqualToExpression()
        {
            var program = this.ParseProgram("foo = (bar >= baz);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var comp = this.AssertNode<GreaterThanOrEqualToExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", comp.Operands[0]);
            this.AssertNode<Identifier>("baz", comp.Operands[1]);
        }

        [TestMethod]
        public void ParseInExpression()
        {
            var program = this.ParseProgram("foo = (bar in baz);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var inExpr = this.AssertNode<InExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", inExpr.Operands[0]);
            this.AssertNode<Identifier>("baz", inExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseInstanceOfExpression()
        {
            var program = this.ParseProgram("foo = (bar instanceof baz);");

            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var inExpr = this.AssertNode<InstanceOfExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", inExpr.Operands[0]);
            this.AssertNode<Identifier>("baz", inExpr.Operands[1]);
        }

        [TestMethod]
        public void ParseConditionalExpression()
        {
            var program = this.ParseProgram("foo = bar ? bar : baz;");
            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var condExpr = this.AssertNode<ConditionalExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", condExpr.Condition);
            this.AssertNode<Identifier>("bar", condExpr.TrueExpression);
            this.AssertNode<Identifier>("baz", condExpr.FalseExpression);
        }

        [TestMethod]
        public void ParsePrefixIncrementer()
        {
            var program = this.ParseProgram("++foo;");
            var incrementer = this.AssertNode<PrefixIncrementExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", incrementer.Operand);
        }

        [TestMethod]
        public void ParsePostfixIncrementer()
        {
            var program = this.ParseProgram("foo++;");
            var incrementer = this.AssertNode<PostfixIncrementExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", incrementer.Operand);
        }

        [TestMethod]
        public void ParsePrefixDecrementer()
        {
            var program = this.ParseProgram("--foo;");
            var incrementer = this.AssertNode<PrefixDecrementExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", incrementer.Operand);
        }

        [TestMethod]
        public void ParsePostfixDecrementer()
        {
            var program = this.ParseProgram("foo--;");
            var incrementer = this.AssertNode<PostfixDecrementExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", incrementer.Operand);
        }

        [TestMethod]
        public void ParsePlusEqualsExpression()
        {
            var program = this.ParseProgram("foo += 3;");

            var assignment = this.AssertNode<AddAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseMinusEqualsExpression()
        {
            var program = this.ParseProgram("foo -= 3;");

            var assignment = this.AssertNode<SubtractAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseTimesEqualsExpression()
        {
            var program = this.ParseProgram("foo *= 3;");

            var assignment = this.AssertNode<MultiplyAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseDivideEqualsExpression()
        {
            var program = this.ParseProgram("foo /= 3;");

            var assignment = this.AssertNode<DivideAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseModEqualsExpression()
        {
            var program = this.ParseProgram("foo %= 3;");

            var assignment = this.AssertNode<ModulusAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseBitwiseAndAssignExpression()
        {
            var program = this.ParseProgram("foo &= 3;");

            var assignment = this.AssertNode<BitwiseAndAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseBitwiseOrAssignExpression()
        {
            var program = this.ParseProgram("foo |= 3;");

            var assignment = this.AssertNode<BitwiseOrAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseBitwiseXorAssignExpression()
        {
            var program = this.ParseProgram("foo ^= 3;");

            var assignment = this.AssertNode<BitwiseXorAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseShiftLeftEqualsExpression()
        {
            var program = this.ParseProgram("foo <<= 3;");

            var assignment = this.AssertNode<ShiftLeftAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseShiftRightEqualsExpression()
        {
            var program = this.ParseProgram("foo >>= 3;");

            var assignment = this.AssertNode<ShiftRightAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseRotateRightEqualsExpression()
        {
            var program = this.ParseProgram("foo >>>= 3;");

            var assignment = this.AssertNode<RotateRightAndAssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);
            this.AssertNode<NumericLiteral>(assignment.Value);
        }

        [TestMethod]
        public void ParseIndexExpression()
        {
            var program = this.ParseProgram("foo = bar[0];");
            var assignment = this.AssertNode<AssignmentExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var indexExpr = this.AssertNode<IndexExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", indexExpr.Object);
            this.AssertNode<NumericLiteral>("0", indexExpr.Index);
        }

        [TestMethod]
        public void ParsePropertyExpression()
        {
            var program = this.ParseProgram("foo.bar();");
            var call = this.AssertNode<CallExpression>(program.Statements[0]);
            var propExpr = this.AssertNode<PropertyExpression>(call.Function);
            this.AssertNode<Identifier>("foo", propExpr.Object);
            this.AssertNode<Identifier>("bar", propExpr.Property);
        }

        [TestMethod]
        public void ParseNewExpression()
        {
            var program = this.ParseProgram("foo = new bar(3);");
            var assignment = this.AssertNode<AssignExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", assignment.Target);

            var newExpr = this.AssertNode<NewExpression>(assignment.Value);
            this.AssertNode<Identifier>("bar", newExpr.Constructor);
            this.AssertNode<NumericLiteral>("3", newExpr.Arguments[0]);
        }

        [TestMethod]
        public void ParseDeleteExpression()
        {
            var program = this.ParseProgram("delete foo;");
            var deletion = this.AssertNode<DeleteExpression>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", deletion.Operand);
        }

        [TestMethod]
        public void ParseEmptyStatement()
        {
            var program = this.ParseProgram(";");
            var stmt = this.AssertNode<EmptyStatement>(program.Statements[0]);
        }

        [TestMethod]
        public void ParseStatementBlock()
        {
            var program = this.ParseProgram("{ bar = baz; }");
            var block = this.AssertNode<StatementBlock>(program.Statements[0]);
            var assignment = this.AssertNode<AssignExpression>(block.Statements[0]);
            this.AssertNode<Identifier>("bar", assignment.Target);
            this.AssertNode<Identifier>("baz", assignment.Value);
        }

        [TestMethod]
        public void ParseVariableStatement()
        {
            var program = this.ParseProgram("var foo;");
            var stmt = this.AssertNode<VariableStatement>(program.Statements[0]);
            var decl = this.AssertNode<VariableDeclaration>(stmt.Declarations[0]);
            this.AssertNode<Identifier>("foo", decl.Identifier);
        }

        [TestMethod]
        public void ParseVariableStatementWithInitializer()
        {
            var program = this.ParseProgram("var foo = 3;");
            var stmt = this.AssertNode<VariableStatement>(program.Statements[0]);
            var decl = this.AssertNode<VariableDeclaration>(stmt.Declarations[0]);
            this.AssertNode<Identifier>("foo", decl.Identifier);
            this.AssertNode<NumericLiteral>("3", decl.InitialValue);
        }

        [TestMethod]
        public void ParseIfStatement()
        {
            var program = this.ParseProgram("if (foo) { bar(3); }");
            var ifStmt = this.AssertNode<IfStatement>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", ifStmt.Condition);
            var block = this.AssertNode<StatementBlock>(ifStmt.Statement);
            var call = this.AssertNode<CallExpression>(block.Statements[0]);
            this.AssertNode<Identifier>("bar", call.Function);
            this.AssertNode<NumericLiteral>("3", call.Arguments[0]);
            Assert.IsNull(ifStmt.ElseStatement);
        }

        [TestMethod]
        public void ParseIfElseStatement()
        {
            var program = this.ParseProgram("if (foo) bar(3); else alert('baz');");
            var ifStmt = this.AssertNode<IfStatement>(program.Statements[0]);
            this.AssertNode<Identifier>("foo", ifStmt.Condition);
            
            var call = this.AssertNode<CallExpression>(ifStmt.Statement);
            this.AssertNode<Identifier>("bar", call.Function);
            this.AssertNode<NumericLiteral>("3", call.Arguments[0]);

            call = this.AssertNode<CallExpression>(ifStmt.ElseStatement);
            this.AssertNode<Identifier>("alert", call.Function);
            this.AssertNode<StringLiteral>("'baz'", call.Arguments[0]);
        }

        [TestMethod]
        public void ParseSwitchStatement()
        {
            var program = this.ParseProgram("switch (foo) { case 1: bar(3); break; case 2: baz(3); break; default: alert('foo!'); }");
            var switchStmt = this.AssertNode<SwitchStatement>(program.Statements[0]);
            
            this.AssertNode<Identifier>("foo", switchStmt.Expression);
            this.AssertNode<NumericLiteral>("1", switchStmt.CaseClauses[0].Expression);
            this.AssertNode<NumericLiteral>("2", switchStmt.CaseClauses[1].Expression);

            var call = this.AssertNode<CallExpression>(switchStmt.CaseClauses[0].Statements[0]);
            this.AssertNode<Identifier>("bar", call.Function);
            this.AssertNode<NumericLiteral>("3", call.Arguments[0]);
            this.AssertNode<BreakStatement>(switchStmt.CaseClauses[0].Statements[1]);

            call = this.AssertNode<CallExpression>(switchStmt.CaseClauses[1].Statements[0]);
            this.AssertNode<Identifier>("baz", call.Function);
            this.AssertNode<NumericLiteral>("3", call.Arguments[0]);
            this.AssertNode<BreakStatement>(switchStmt.CaseClauses[1].Statements[1]);

            call = this.AssertNode<CallExpression>(switchStmt.DefaultClause.Statements[0]);
            this.AssertNode<Identifier>("alert", call.Function);
            this.AssertNode<StringLiteral>("'foo!'", call.Arguments[0]);
        }

        [TestMethod]
        public void ParseForLoop()
        {
            var program = this.ParseProgram("for (var i = 0; i < foo; i++) bar(i);");
            var forStmt = this.AssertNode<ForStatement>(program.Statements[0]);

            var varStmt = this.AssertNode<VariableStatement>(forStmt.InitializationStatement);
            this.AssertNode<Identifier>("i", varStmt.Declarations[0].Identifier);
            this.AssertNode<NumericLiteral>("0", varStmt.Declarations[0].InitialValue);

            var comp = this.AssertNode<LessThanExpression>(forStmt.Condition);
            this.AssertNode<Identifier>("i", comp.Operands[0]);
            this.AssertNode<Identifier>("foo", comp.Operands[1]);

            var incr = this.AssertNode<PostfixIncrementExpression>(forStmt.IncrementStatement);
            this.AssertNode<Identifier>("i", incr.Operand);

            var call = this.AssertNode<CallExpression>(forStmt.LoopStatement);
            this.AssertNode<Identifier>("bar", call.Function);
            this.AssertNode<Identifier>("i", call.Arguments[0]);
        }

        [TestMethod]
        public void ParseForInStatement()
        {
            var program = this.ParseProgram("for (var foo in bar) { foo++; }");
            var stmt = this.AssertNode<ForInStatement>(program.Statements[0]);
            
            var decl = this.AssertNode<VariableDeclaration>(stmt.LoopVariable);
            this.AssertNode<Identifier>("foo", decl.Identifier);
            this.AssertNode<Identifier>("bar", stmt.Object);

            var block = this.AssertNode<StatementBlock>(stmt.LoopStatement);
            var incr = this.AssertNode<PostfixIncrementExpression>(block.Statements[0]);
            this.AssertNode<Identifier>("foo", incr.Operand);
        }

        [TestMethod]
        public void ParseWhileLoop()
        {
            var program = this.ParseProgram("while (true) { foo(); }");
            var loop = this.AssertNode<WhileStatement>(program.Statements[0]);
            this.AssertNode<TrueLiteral>(loop.Condition);

            var block = this.AssertNode<StatementBlock>(loop.Statement);
            var call = this.AssertNode<CallExpression>(block.Statements[0]);
            this.AssertNode<Identifier>("foo", call.Function);
        }

        [TestMethod]
        public void ParseDoWhileLoop()
        {
            var program = this.ParseProgram("do { foo.bar(); } while(foo.baz);");
            var doStmt = this.AssertNode<DoWhileStatement>(program.Statements[0]);

            var block = this.AssertNode<StatementBlock>(doStmt.Statement);
            var callExpr = this.AssertNode<CallExpression>(block.Statements[0]);
            var propExpr = this.AssertNode<PropertyExpression>(callExpr.Function);
            this.AssertNode<Identifier>("foo", propExpr.Object);
            this.AssertNode<Identifier>("bar", propExpr.Property);

            propExpr = this.AssertNode<PropertyExpression>(doStmt.Condition);
            this.AssertNode<Identifier>("foo", propExpr.Object);
            this.AssertNode<Identifier>("baz", propExpr.Property);
        }

        [TestMethod]
        public void ParseContinueStatement()
        {
            var program = this.ParseProgram("for (var i = 1; i <= numFloors; i++) { if (i == 13) continue; alert(i); }");
            var forLoop = this.AssertNode<ForStatement>(program.Statements[0]);
            var block = this.AssertNode<StatementBlock>(forLoop.LoopStatement);
            var ifStmt = this.AssertNode<IfStatement>(block.Statements[0]);
            this.AssertNode<ContinueStatement>(ifStmt.Statement);
        }

        [TestMethod]
        public void ParseThrowStatement()
        {
            var program = this.ParseProgram("throw new NotImplementedException();");
            var throwStmt = this.AssertNode<ThrowStatement>(program.Statements[0]);
            var newExpr = this.AssertNode<NewExpression>(throwStmt.Expression);
            this.AssertNode<Identifier>("NotImplementedException", newExpr.Constructor);
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

            var labeledStmt = this.AssertNode<LabeledStatement>(program.Statements[0]);
            this.AssertNode<Identifier>("outer", labeledStmt.Label);

            var loop = this.AssertNode<ForInStatement>(labeledStmt.Statement);
            this.AssertNode<Identifier>("foo", loop.LoopVariable);
            this.AssertNode<Identifier>("bar", loop.Object);

            var block = this.AssertNode<StatementBlock>(loop.LoopStatement);
            loop = this.AssertNode<ForInStatement>(block.Statements[0]);
            this.AssertNode<Identifier>("baz", loop.LoopVariable);
            var indexExpr = this.AssertNode<IndexExpression>(loop.Object);
            this.AssertNode<Identifier>("bar", indexExpr.Object);
            this.AssertNode<Identifier>("foo", indexExpr.Index);

            block = this.AssertNode<StatementBlock>(loop.LoopStatement);
            var ifStmt = this.AssertNode<IfStatement>(block.Statements[0]);
            var equalsExpr = this.AssertNode<EqualToExpression>(ifStmt.Condition);
            this.AssertNode<Identifier>("baz", equalsExpr.Operands[0]);
            this.AssertNode<StringLiteral>("'name'", equalsExpr.Operands[1]);

            block = this.AssertNode<StatementBlock>(ifStmt.Statement);
            var assignExpr = this.AssertNode<AssignExpression>(block.Statements[0]);
            this.AssertNode<Identifier>("name", assignExpr.Target);

            indexExpr = this.AssertNode<IndexExpression>(assignExpr.Value);
            this.AssertNode<Identifier>("baz", indexExpr.Index);
            indexExpr = this.AssertNode<IndexExpression>(indexExpr.Object);
            this.AssertNode<Identifier>("foo", indexExpr.Index);
            this.AssertNode<Identifier>("bar", indexExpr.Object);

            var breakStmt = this.AssertNode<BreakStatement>(block.Statements[1]);
            this.AssertNode<Identifier>("outer", breakStmt.Label);
        }

        [TestMethod]
        public void ParseTryStatement()
        {
            var program = this.ParseProgram("try { foo(); } catch (ex) { bar(ex); } finally { baz(); }");
            var tryStmt = this.AssertNode<TryStatement>(program.Statements[0]);

            var call = this.AssertNode<CallExpression>(tryStmt.Statements[0]);
            this.AssertNode<Identifier>("foo", call.Function);

            Assert.AreEqual("ex", tryStmt.CatchClause.Identifier.Text);
            call = this.AssertNode<CallExpression>(tryStmt.CatchClause.Statements[0]);
            this.AssertNode<Identifier>("bar", call.Function);
            this.AssertNode<Identifier>("ex", call.Arguments[0]);

            call = this.AssertNode<CallExpression>(tryStmt.FinallyClause.Statements[0]);
            this.AssertNode<Identifier>("baz", call.Function);
        }

        [TestMethod]
        public void ParseWithStatement()
        {
            var program = this.ParseProgram("with (foo) { bar(); baz(); }");
            var withStmt = this.AssertNode<WithStatement>(program.Statements[0]);
            var block = this.AssertNode<StatementBlock>(withStmt.Statement);
            this.AssertNode<Identifier>("foo", withStmt.Expression);
            
            var call = this.AssertNode<CallExpression>(block.Statements[0]);
            this.AssertNode<Identifier>("bar", call.Function);

            call = this.AssertNode<CallExpression>(block.Statements[1]);
            this.AssertNode<Identifier>("baz", call.Function);
        }

        [TestMethod]
        public void ParseFunctionDeclaration()
        {
            var program = this.ParseProgram("function foo(bar, baz) { bar = baz; }");
            var decl = this.AssertNode<FunctionDeclaration>(program.Children[0]);

            this.AssertNode<Identifier>("foo", decl.Children[0]);

            var paramList = this.AssertNode<FormalParameterList>(decl.Children[1]);
            this.AssertNode<Identifier>("bar", paramList.Children[0]);
            this.AssertNode<Identifier>("baz", paramList.Children[1]);

            var body = this.AssertNode<FunctionBody>(decl.Children[2]);
            var assignment = this.AssertNode<AssignExpression>(body.Children[0]);
            this.AssertNode<Identifier>("bar", assignment.Children[0]);
            this.AssertNode<Identifier>("baz", assignment.Children[1]);
        }

        private Program ParseProgram(string code)
        {
            var charStream = new ANTLRStringStream(code);
            var tokenSource = new JavaScriptLexer(charStream);
            var tokenStream = new CommonTokenStream(tokenSource);
            var parser = new JavaScriptParser(tokenStream);
            var program = parser.program().Tree as Program;
            return program;
        }

        private void AssertNode<T>(string expectedText, ITree tree) where T : CommonTree
        {
            Assert.AreEqual(expectedText, this.AssertNode<T>(tree).Text);
        }

        private T AssertNode<T>(object obj) where T : CommonTree
        {
            Assert.IsInstanceOfType(obj, typeof(T));
            return (T)obj;
        }
    }
}
