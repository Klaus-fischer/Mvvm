using System.Linq.Expressions;

namespace SIM.Mvvm.CodeGeneration.Tests
{
    public partial class AutoMapExceptViewModel : ViewModel
    {
        [AutoMapProperties(nameof(Model.Age))]
        public Model Data { get; } = new Model();

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
