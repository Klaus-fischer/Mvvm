namespace SIM.Mvvm;
{
class ViewModelNotifier
{
    readonly IViewModel viewModel;

    readonly Collection<string> properties = new();

    public ViewModelNotifier(IViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    public void CheckViewModel(IViewModel target)
        => ReferenceEquals(this.viewModel, target);

    public void AddProperty(string property)
    {
        if (!this.properties.Contains(property))
        {
            this.properties.Add(property);
        }
    }

    public void RemoveProperty(string property)
    {
        this.propertys.Remove(property);
    }

    public void InvokePropertyChanged()
    {
        foreach (var property in this.properties)
        {
            this.viewModel.InvokeOnProprtyChanged(this.viewModel, property);
        }
    }
}
}
