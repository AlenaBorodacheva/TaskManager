﻿using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using TaskManager.Client.Models;
using TaskManager.Client.Views.AddWindows;
using TaskManager.Common.Models;

namespace TaskManager.Client.Services;

public class DesksViewService
{
    private DesksRequestService _desksRequestService;
    private AuthToken _token;
    public CommonViewService ViewService { get; set; }

    public DesksViewService(AuthToken token, DesksRequestService desksRequestService)
    {
        _token = token;
        _desksRequestService = desksRequestService;
        ViewService = new CommonViewService();
    }

    public ModelClient<DeskModel> GetDeskClientById(object deskId)
    {
        try
        {
            int id = (int)deskId;
            DeskModel desk = _desksRequestService.GetDeskById(_token, id);
            return new ModelClient<DeskModel>(desk);
        }
        catch (FormatException)
        {
            return new ModelClient<DeskModel>(null);
        }
    }

    public List<ModelClient<DeskModel>> GetDesks(int projectId)
    {
        var result = new List<ModelClient<DeskModel>>();
        var desks = _desksRequestService.GetDesksByProject(_token, projectId);
        if (desks != null)
        {
            result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();
        }

        return result;
    }

    public List<ModelClient<DeskModel>> GetAllDesks()
    {
        var result = new List<ModelClient<DeskModel>>();
        var desks = _desksRequestService.GetAllDesks(_token);
        if (desks != null)
        {
            result = desks.Select(d => new ModelClient<DeskModel>(d)).ToList();
        }

        return result;
    }

    public void OpenViewDeskInfo(object deskId, BindableBase context)
    {
        var wnd = new CreateOrUpdateDeskWindow();
        ViewService.OpenWindow(wnd, context);        
    }

    public void UpdateDesk(DeskModel project)
    {
        var resultAction = _desksRequestService.UpdateDesk(_token, project);
        ViewService.ShowActionResult(resultAction, "\nDesk is updated");
    }

    public void DeleteDesk(int deskId)
    {
        var resultAction = _desksRequestService.DeleteDesk(_token, deskId);
        ViewService.ShowActionResult(resultAction, "\nDesk is deleted");
        ViewService.CurrentOpenWindow?.Close();
    }

    public ModelClient<DeskModel> SelectPhotoForDesk(ModelClient<DeskModel> selectedDesk)
    {
        if(selectedDesk?.Model != null)
        {
            ViewService.SetPhotoForObject(selectedDesk.Model);
            selectedDesk = new ModelClient<DeskModel>(selectedDesk.Model);
        }  
        return selectedDesk;
    }
}