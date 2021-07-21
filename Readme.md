# SIM MVVM
Standart Bibliothek für alle Model-View-ViewModel implementationen.
## namespace SIM.Mvvm
### SIM.Mvvm.ViewModel : IViewModel
- Basisklasse für  die MVVM funktionalität.
##### Aufruf für PropertyChanged:
 
    OnPropertyChanged(string? propertyName, object? before, object? after)

##### Eigenschaften implementieren:
    private int _property;
    public int Property
    {
        get => this._property;
        set => this.SetPropertyValue(ref _property, value);
    }

#### Vordefinierte Commands:
* [Command](../SIM.Mvvm/Commands/Abstract/Command.cs): Basisklasse für parameterlose Commands
** [RelayCommand](../SIM.Mvvm/Commands/RelayCommand.cs): nutzt delegates um die Funktionalität zu Implementieren
** [AsyncRelayCommand](../SIM.Mvvm/Commands/AsyncRelayCommand.cs): asynchrone variante von RelayCommand
** [EventCommand]((../SIM.Mvvm/Commands/EventCommand.cs): nutzt events um die Funktionalität zu Implementieren

* [ParameterCommand\<T\>](../SIM.Mvvm/Commands/Abstract/ParameterCommand\{T\}.cs): Basisklasse für  Commands mit Parametern
** [RelayCommand\<T\>](../SIM.Mvvm/Commands/RelayCommand\{T\}.cs) - nutzt delegates um die Funktionalität zu Implementieren
** [AsyncRelayCommand\<T\>](../SIM.Mvvm/Commands/AsyncRelayCommand\{T\}.cs) - asynchrone variante von RelayCommand\<T\>
** [EventCommand\<T\>](../SIM.Mvvm/Commands/EventCommand\{T\}.cs) - nutzt events um die Funktionalität zu Implementieren

