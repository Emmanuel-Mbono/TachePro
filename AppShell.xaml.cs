using TachePro.Views;

namespace TachePro;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Écrans empilés (ouverts par-dessus la barre d'onglets)
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("taskdetail", typeof(TaskDetailPage));
        Routing.RegisterRoute("taskedit", typeof(TaskEditPage));
    }
}
