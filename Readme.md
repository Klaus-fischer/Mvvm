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
- Command - Basisklasse für parameterlose Commands
-- RelayCommand - nutzt delegates um die Funktionalität zu Implementieren
-- AsyncRelayCommand - asynchrone variante von RelayCommand
-- EventCommand - nutzt events um die Funktionalität zu Implementieren

- ParameterCommand\<T\> - Basisklasse für  Commands mit Parametern
-- RelayCommand\<T\> - nutzt delegates um die Funktionalität zu Implementieren
-- AsyncRelayCommand\<T\> - asynchrone variante von RelayCommand\<T\>
-- EventCommand\<T\> - nutzt events um die Funktionalität zu Implementieren

