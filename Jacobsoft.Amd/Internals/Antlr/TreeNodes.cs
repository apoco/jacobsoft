using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Jacobsoft.Amd.Internals.AntlrGenerated
{
    internal abstract class BaseNode : CommonTree
    {
        public BaseNode(int nodeType) : base(new CommonToken(nodeType)) { }
        public BaseNode(IToken token) : base(token) { }

        protected T GetOptional<T>(int childIndex) where T : BaseNode
        {
            return this.Children.Count > childIndex 
                ? this.Children[childIndex] as T 
                : null;
        }
    }

    internal class Program : BaseNode
    {
        public Program(int nodeType) : base(nodeType) { }

        public IList<ITree> Statements
        {
            get { return base.Children; }
        }
    }

    #region Statements

    internal abstract class Statement : BaseNode
    {
        public Statement(int nodeType) : base(nodeType) { }
        public Statement(IToken token) : base(token) { }
    }

    internal class FunctionDeclaration : Statement
    {
        public FunctionDeclaration(IToken token) : base(token) { }

        public StatementBlock StatementBlock
        {
            get { return this.Children[2] as StatementBlock; }
        }

        public IList<Statement> Statements
        {
            get { return this.StatementBlock.Statements; }
        }
    }

    internal class FormalParameterList : BaseNode
    {
        private readonly Lazy<IList<Identifier>> parameters;

        public FormalParameterList(IToken token) : base(token) 
        {
            this.parameters = new Lazy<IList<Identifier>>(
                () => new AutoCastingList<Identifier, ITree>(this.Children));
        }

        public IList<Identifier> Parameters
        {
            get { return this.parameters.Value; }
        }
    }

    internal class StatementBlock : Statement
    {
        private readonly Lazy<IList<Statement>> statements;

        public StatementBlock(IToken token) : base(token) 
        {
            this.statements = new Lazy<IList<Statement>>(
                () => new AutoCastingList<Statement, ITree>(this.Children));
        }

        public IList<Statement> Statements 
        {
            get { return this.statements.Value; }
        }
    }

    internal class VariableStatement : Statement
    {
        private readonly Lazy<IList<VariableDeclaration>> declarations;

        public VariableStatement(IToken token) : base(token) 
        {
            this.declarations = new Lazy<IList<VariableDeclaration>>(() =>
                new AutoCastingList<VariableDeclaration, ITree>(this.Children));
        }

        public IList<VariableDeclaration> Declarations 
        {
            get { return this.declarations.Value; } 
        }
    }

    internal class VariableDeclaration : BaseNode
    {
        public VariableDeclaration(int nodeType) : base(nodeType) { }

        public Identifier Identifier
        {
            get { return this.Children[0] as Identifier; }
        }

        public Expression InitialValue
        {
            get { return this.GetOptional<Expression>(1); }
        }
    }

    internal class EmptyStatement : Statement
    {
        public EmptyStatement(IToken token) : base(token) { }
    }

    internal class IfStatement : Statement
    {
        public IfStatement(IToken token) : base(token) { }

        public Expression Condition
        {
            get { return this.Children[0] as Expression; }
        }

        public Statement Statement
        {
            get { return this.Children[1] as Statement; }
        }

        public Statement ElseStatement
        {
            get { return this.GetOptional<Statement>(2); }
        }
    }

    internal class DoWhileStatement : Statement
    {
        public DoWhileStatement(IToken token) : base(token) { }

        public Statement Statement
        {
            get { return this.Children[0] as Statement; }
        }

        public Expression Condition
        {
            get { return this.Children[1] as Expression; }
        }
    }

    internal class WhileStatement : Statement
    {
        public WhileStatement(IToken token) : base(token) { }

        public Expression Condition
        {
            get { return this.Children[0] as Expression; }
        }

        public Statement Statement
        {
            get { return this.Children[1] as Statement; }
        }
    }

    internal class ForStatement : Statement
    {
        public ForStatement(IToken token) : base(token) { }

        public BaseNode InitializationStatement
        {
            get 
            {
                return (this.Children[0] as ForStatementInitializer).Statement;
            }
        }

        public Expression Condition
        {
            get
            {
                return (this.Children[1] as ForStatementCondition).Condition;
            }
        }

        public Statement IncrementStatement
        {
            get
            {
                return (this.Children[2] as ForStatementIncrementer).Statement;
            }
        }

        public Statement LoopStatement
        {
            get { return this.Children[3] as Statement; }
        }
    }

    internal class ForStatementInitializer : BaseNode
    {
        public ForStatementInitializer(int nodeType) : base(nodeType) { }

        public BaseNode Statement
        {
            get { return this.Children.FirstOrDefault() as BaseNode; }
        }
    }

    internal class ForStatementCondition : BaseNode
    {
        public ForStatementCondition(int nodeType) : base(nodeType) { }

        public Expression Condition
        {
            get { return this.Children.FirstOrDefault() as Expression; }
        }
    }

    internal class ForStatementIncrementer : BaseNode
    {
        public ForStatementIncrementer(int nodeType) : base(nodeType) { }

        public Statement Statement
        {
            get { return this.Children.FirstOrDefault() as Statement; }
        }
    }

    internal class ForInStatement : Statement
    {
        public ForInStatement(IToken token) : base(token) { }

        public BaseNode LoopVariable
        {
            get { return this.Children[0] as BaseNode; }
        }

        public Expression Object
        {
            get { return this.Children[1] as Expression; }
        }

        public Statement LoopStatement
        {
            get { return this.Children[2] as Statement; }
        }
    }

    internal class ContinueStatement : Statement
    {
        public ContinueStatement(IToken token) : base(token) { }
    }

    internal class BreakStatement : Statement
    {
        public BreakStatement(IToken token) : base(token) { }

        public Identifier Label
        {
            get { return this.Children.OfType<Identifier>().LastOrDefault(); }
        }
    }

    internal class ReturnStatement : Statement
    {
        public ReturnStatement(IToken token) : base(token) { }
    }

    internal class WithStatement : Statement
    {
        public WithStatement(IToken token) : base(token) { }

        public Expression Expression
        {
            get { return this.Children[0] as Expression; }
        }

        public Statement Statement
        {
            get { return this.Children[1] as Statement; }
        }
    }

    internal class LabeledStatement : Statement
    {
        public LabeledStatement(IToken token) : base(token) { }

        public Identifier Label
        {
            get { return this.Children[0] as Identifier; }
        }

        public Statement Statement
        {
            get { return this.Children[1] as Statement; }
        }
    }

    internal class SwitchStatement : Statement
    {
        private readonly Lazy<IList<CaseClause>> caseClauses;

        public SwitchStatement(IToken token) : base(token) 
        {
            this.caseClauses = new Lazy<IList<CaseClause>>(
                () => this.Children.OfType<CaseClause>().ToList());
        }

        public Expression Expression
        {
            get { return this.Children[0] as Expression; }
        }

        public IList<CaseClause> CaseClauses
        {
            get { return this.caseClauses.Value; }
        }

        public DefaultCaseClause DefaultClause
        {
            get 
            {
                return this.Children.Last() as DefaultCaseClause; 
            }
        }
    }

    internal abstract class SwitchClause : BaseNode
    {
        public SwitchClause(IToken token) : base(token) { }
        
        public IList<Statement> Statements 
        { 
            get { return this.GetStatements(); }
        }

        protected abstract IList<Statement> GetStatements();
    }

    internal class CaseClause : SwitchClause
    {
        public CaseClause(IToken token) : base(token) { }

        public Expression Expression
        {
            get { return this.Children[0] as Expression; }
        }

        protected override IList<Statement> GetStatements()
        {
            return this.Children.Skip(1).Cast<Statement>().ToList();
        }
    }

    internal class DefaultCaseClause : SwitchClause
    {
        public DefaultCaseClause(IToken token) : base(token) { }

        protected override IList<Statement> GetStatements()
        {
            return new AutoCastingList<Statement, ITree>(this.Children);
        }
    }

    internal class ThrowStatement : Statement
    {
        public ThrowStatement(IToken token) : base(token) { }

        public Expression Expression
        {
            get { return this.Children[0] as Expression; }
        }
    }

    internal class TryStatement : Statement
    {
        private readonly Lazy<IList<Statement>> statements;
        private readonly Lazy<CatchClause> catchClause;
        private readonly Lazy<FinallyClause> finallyClause;

        public TryStatement(IToken token) : base(token) 
        {
            this.statements = new Lazy<IList<Statement>>(
                () => (this.Children[0] as StatementBlock).Statements);
            this.catchClause = new Lazy<CatchClause>(
                () => this.Children.OfType<CatchClause>().FirstOrDefault());
            this.finallyClause = new Lazy<FinallyClause>(
                () => this.Children.Last() as FinallyClause);
        }

        public IList<Statement> Statements
        {
            get { return this.statements.Value; }
        }

        public CatchClause CatchClause
        {
            get { return this.catchClause.Value; }
        }

        public FinallyClause FinallyClause
        {
            get { return this.finallyClause.Value; }
        }
    }

    internal class CatchClause : BaseNode
    {
        public CatchClause(IToken token) : base(token) { }

        public Identifier Identifier
        {
            get { return this.Children[0] as Identifier; }
        }

        public IList<Statement> Statements
        {
            get { return (this.Children[1] as StatementBlock).Statements; }
        }
    }

    internal class FinallyClause : BaseNode
    {
        public FinallyClause(IToken token) : base(token) { }

        public IList<Statement> Statements
        {
            get { return (this.Children[0] as StatementBlock).Statements; }
        }
    }

    #endregion

    #region Expressions

    internal abstract class Expression : Statement
    {
        public Expression(int nodeType) : base(nodeType) { }
        public Expression(IToken token) : base(token) { }
    }

    internal class Identifier : Expression
    {
        public Identifier(IToken token) : base(token) { }
    }

    internal class FunctionExpression : Expression
    {
        public FunctionExpression(IToken token) : base(token) { }

        public IList<Identifier> Parameters
        {
            get { return this.Children.OfType<FormalParameterList>().First().Parameters; }
        }

        public StatementBlock StatementBlock
        {
            get { return this.Children.OfType<StatementBlock>().First(); }
        }

        public IList<Statement> Statements
        {
            get { return this.StatementBlock.Statements; }
        }
    }

    internal abstract class UnaryExpression : Expression
    {
        public UnaryExpression(IToken token) : base(token) { }

        public Expression Operand
        {
            get { return this.Children[0] as Expression; }
        }
    }

    internal class DeleteExpression : UnaryExpression
    {
        public DeleteExpression(IToken token) : base(token) { }
    }

    internal class VoidExpression : UnaryExpression
    {
        public VoidExpression(IToken token) : base(token) { }
    }

    internal class TypeofExpression : UnaryExpression
    {
        public TypeofExpression(IToken token) : base(token) { }
    }

    internal class PrefixIncrementExpression : UnaryExpression
    {
        public PrefixIncrementExpression(IToken token) : base(token) { }
    }

    internal class PrefixDecrementExpression : UnaryExpression
    {
        public PrefixDecrementExpression(IToken token) : base(token) { }
    }

    internal class UnaryPlusExpression : UnaryExpression
    {
        public UnaryPlusExpression(IToken token) : base(token) { }
    }

    internal class NegationExpression : UnaryExpression
    {
        public NegationExpression(IToken token) : base(token) { }
    }

    internal class BitwiseNotExpression : UnaryExpression
    {
        public BitwiseNotExpression(IToken token) : base(token) { }
    }

    internal class BooleanNotExpression : UnaryExpression
    {
        public BooleanNotExpression(IToken token) : base(token) { }
    }

    internal class PostfixIncrementExpression : UnaryExpression
    {
        public PostfixIncrementExpression(IToken token) : base(token) { }
    }

    internal class PostfixDecrementExpression : UnaryExpression
    {
        public PostfixDecrementExpression(IToken token) : base(token) { }
    }

    internal class ThisExpression : Expression
    {
        public ThisExpression(IToken token) : base(token) { }
    }

    internal class NewExpression : Expression
    {
        public NewExpression(IToken token) : base(token) { }

        public Expression Constructor
        {
            get { return this.Children[0] as Expression; }
        }

        public IList<Expression> Arguments
        {
            get { return (this.Children[1] as ArgumentList).Arguments; }
        }
    }

    internal class CallExpression : Expression
    {
        public CallExpression(int nodeType) : base(nodeType) { }

        public Expression Function
        {
            get { return this.Children[0] as Expression; }
        }

        public ArgumentList ArgumentList
        {
            get { return this.Children[1] as ArgumentList; }
        }

        public IList<Expression> Arguments 
        {
            get { return this.ArgumentList.Arguments; }
        }
    }

    internal class ArgumentList : BaseNode
    {
        private readonly Lazy<IList<Expression>> arguments;

        public ArgumentList(IToken token) : base(token)
        {
            this.arguments = new Lazy<IList<Expression>>(() => new AutoCastingList<Expression, ITree>(
                this.Children));
        }

        public IList<Expression> Arguments
        {
            get { return this.arguments.Value; }
        }
    }

    internal class IndexExpression : Expression
    {
        public IndexExpression(int nodeType) : base(nodeType) { }

        public Expression Object
        {
            get { return this.Children[0] as Expression; }
        }

        public Expression Index
        {
            get { return this.Children[1] as Expression; }
        }
    }

    internal class PropertyExpression : Expression
    {
        public PropertyExpression(int nodeType) : base(nodeType) { }

        public Expression Object
        {
            get { return this.Children[0] as Expression; }
        }

        public Identifier Property
        {
            get { return this.Children[1] as Identifier; }
        }
    }

    internal class ConditionalExpression : Expression
    {
        public ConditionalExpression(IToken token) : base(token) { }

        public Expression Condition
        {
            get { return this.Children[0] as Expression; }
        }

        public Expression TrueExpression
        {
            get { return this.Children[1] as Expression; }
        }

        public Expression FalseExpression
        {
            get { return this.Children[2] as Expression; }
        }
    }

    #region Binary Operations

    internal abstract class BinaryOperationExpression : Expression
    {
        private readonly Lazy<IList<Expression>> operands;

        public BinaryOperationExpression(IToken token) : base(token) 
        {
            this.operands = new Lazy<IList<Expression>>(() =>
                new AutoCastingList<Expression, ITree>(this.Children));
        }

        public IList<Expression> Operands
        {
            get { return this.operands.Value; }
        }
    }

    internal class InExpression : BinaryOperationExpression
    {
        public InExpression(IToken token) : base(token) { }
    }

    internal class LogicalOrExpression : BinaryOperationExpression
    {
        public LogicalOrExpression(IToken token) : base(token) { }
    }

    internal class LogicalAndExpression : BinaryOperationExpression
    {
        public LogicalAndExpression(IToken token) : base(token) { }
    }

    internal class BitwiseOrExpression : BinaryOperationExpression
    {
        public BitwiseOrExpression(IToken token) : base(token) { }
    }

    internal class BitwiseXorExpression : BinaryOperationExpression
    {
        public BitwiseXorExpression(IToken token) : base(token) { }
    }

    internal class BitwiseAndExpression : BinaryOperationExpression
    {
        public BitwiseAndExpression(IToken token) : base(token) { }
    }

    internal class ShiftLeftExpression : BinaryOperationExpression
    {
        public ShiftLeftExpression(IToken token) : base(token) { }
    }

    internal class ShiftRightExpression : BinaryOperationExpression
    {
        public ShiftRightExpression(IToken token) : base(token) { }
    }

    internal class RotateRightExpression : BinaryOperationExpression
    {
        public RotateRightExpression(IToken token) : base(token) { }
    }

    internal class AdditionExpression : BinaryOperationExpression
    {
        public AdditionExpression(IToken token) : base(token) { }
    }

    internal class SubtractionExpression : BinaryOperationExpression
    {
        public SubtractionExpression(IToken token) : base(token) { }
    }

    internal class MultiplicationExpression : BinaryOperationExpression
    {
        public MultiplicationExpression(IToken token) : base(token) { }
    }

    internal class DivisionExpression : BinaryOperationExpression
    {
        public DivisionExpression(IToken token) : base(token) { }
    }

    internal class ModulusExpression : BinaryOperationExpression
    {
        public ModulusExpression(IToken token) : base(token) { }
    }

    #endregion

    #region Comparisons

    internal abstract class ComparisonExpression : BinaryOperationExpression
    {
        public ComparisonExpression(IToken token) : base(token) { }
    }

    internal class EqualToExpression : ComparisonExpression
    {
        public EqualToExpression(IToken token) : base(token) { }
    }

    internal class NotEqualToExpression : ComparisonExpression
    {
        public NotEqualToExpression(IToken token) : base(token) { }
    }

    internal class StrictlyEqualToExpression : ComparisonExpression
    {
        public StrictlyEqualToExpression(IToken token) : base(token) { }
    }

    internal class NotStrictlyEqualToExpression : ComparisonExpression
    {
        public NotStrictlyEqualToExpression(IToken token) : base(token) { }
    }

    internal class LessThanExpression : ComparisonExpression
    {
        public LessThanExpression(IToken token) : base(token) { }
    }

    internal class GreaterThanExpression : ComparisonExpression
    {
        public GreaterThanExpression(IToken token) : base(token) { }
    }

    internal class LessThanOrEqualToExpression : ComparisonExpression
    {
        public LessThanOrEqualToExpression(IToken token) : base(token) { }
    }

    internal class GreaterThanOrEqualToExpression : ComparisonExpression
    {
        public GreaterThanOrEqualToExpression(IToken token) : base(token) { }
    }

    internal class InstanceOfExpression : ComparisonExpression
    {
        public InstanceOfExpression(IToken token) : base(token) { }
    }

    #endregion

    #region Assignments

    internal class AssignmentExpression : Expression
    {
        public AssignmentExpression(int nodeType) : base(nodeType) { }

        public BaseNode Target 
        {
            get { return base.Children[0] as BaseNode; }
        }

        public Expression Value 
        {
            get { return base.Children[1] as Expression; }
        }
    }

    internal class AssignExpression : AssignmentExpression
    {
        public AssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class MultiplyAndAssignExpression : AssignmentExpression
    {
        public MultiplyAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class DivideAndAssignExpression : AssignmentExpression
    {
        public DivideAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class ModulusAndAssignExpression : AssignmentExpression
    {
        public ModulusAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class AddAndAssignExpression : AssignmentExpression
    {
        public AddAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class SubtractAndAssignExpression : AssignmentExpression
    {
        public SubtractAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class ShiftLeftAndAssignExpression : AssignmentExpression
    {
        public ShiftLeftAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class ShiftRightAndAssignExpression : AssignmentExpression
    {
        public ShiftRightAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class RotateRightAndAssignExpression : AssignmentExpression
    {
        public RotateRightAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class BitwiseAndAndAssignExpression : AssignmentExpression
    {
        public BitwiseAndAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class BitwiseXorAndAssignExpression : AssignmentExpression
    {
        public BitwiseXorAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    internal class BitwiseOrAndAssignExpression : AssignmentExpression
    {
        public BitwiseOrAndAssignExpression(int nodeType) : base(nodeType) { }
    }

    #endregion

    #region Literals

    internal class ArrayLiteral : Expression
    {
        private readonly Lazy<IList<Expression>> items;

        public ArrayLiteral(IToken token) : base(token) 
        {
            this.items = new Lazy<IList<Expression>>(
                () => new AutoCastingList<Expression, ITree>(this.Children));
        }

        public IList<Expression> Items
        {
            get { return items.Value; }
        }
    }

    internal class ObjectLiteral : Expression
    {
        private readonly Lazy<IList<PropertyAssignment>> assignments;

        public ObjectLiteral(IToken token) : base(token) 
        {
            this.assignments = new Lazy<IList<PropertyAssignment>>(
                () => new AutoCastingList<PropertyAssignment, ITree>(this.Children));
        }

        public IList<PropertyAssignment> Assignments
        {
            get { return assignments.Value; }
        }
    }

    internal class PropertyAssignment : BaseNode
    {
        public PropertyAssignment(IToken token) : base(token) { }

        public Expression Property
        {
            get { return this.Children[0] as Expression; }
        }

        public Expression Value
        {
            get { return this.Children[1] as Expression; }
        }
    }

    internal class StringLiteral : Expression
    {
        public StringLiteral(IToken token) : base(token) { }

        public string String
        {
            get { return new JavaScriptSerializer().Deserialize<string>(this.Text); }
        }
    }

    internal class NumericLiteral : Expression
    {
        public NumericLiteral(IToken token) : base(token) { }
    }

    internal class NullLiteral : Expression
    {
        public NullLiteral(IToken token) : base(token) { }
    }

    internal class TrueLiteral : Expression
    {
        public TrueLiteral(IToken token) : base(token) { }
    }

    internal class FalseLiteral : Expression
    {
        public FalseLiteral(IToken token) : base(token) { }
    }

    #endregion

    #endregion
}
