using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Services;
using TaskManager.Common.Models;

namespace TaskManager.Client.ViewModels;

public class ProjectDesksPageViewModel  : BindableBase
{
    private CommonViewService _viewService;
    private AuthToken _token;
    private ProjectModel _project;
    private DesksRequestService _desksRequestService;

    private List<ModelClient<DeskModel>> _projectDesks = new List<ModelClient<DeskModel>>();

    public List<ModelClient<DeskModel>> ProjectDesks
    {
        get => _projectDesks;
        set
        {
            _projectDesks = value;
            RaisePropertyChanged(nameof(ProjectDesks));
        }
    }

    public ProjectDesksPageViewModel(AuthToken token, ProjectModel project)
    {
        _token = token;
        _project = project;
        _desksRequestService = new DesksRequestService();

        ProjectDesks = GetDesks();
    }

    #region COMMANDS

    #endregion

    private List<ModelClient<DeskModel>> GetDesks()
    {
        var result = new List<ModelClient<DeskModel>>();
        var desks = _desksRequestService.GetDesksByProject(_token, _project.Id);
        if (desks != null)
        {
            result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();
        }

        return result;
    }
}