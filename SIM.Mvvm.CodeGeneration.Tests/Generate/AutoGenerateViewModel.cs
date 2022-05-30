using System.Linq.Expressions;

namespace SIM.Mvvm.CodeGeneration.Tests
{
    public partial class AutoGenerateViewModel : ViewModel
    {
        [GenerateProperty]
        internal string _TestString = "TestSuccess";

        [GenerateProperty(SetterVisibility.Private)]
        internal bool _testBool;

        [GenerateProperty]
        internal int testInteger = 42;

        public object GetValue(string name)
        {
            return Expression.Lambda(Expression.Property(Expression.Constant(this), name)).Compile().DynamicInvoke();
        }

        public void SetValue<T>(string name, T value)
        {
            Expression.Lambda(
                Expression.Assign(
                    Expression.Property(
                        Expression.Constant(this),
                        name),
                    Expression.Constant(value))).Compile().DynamicInvoke();
        }
    }
}
