using DataAccessLayer.Database;
using DataAccessLayer.Models;
using System;

namespace ManagerLayer.LaunchManagement
{
    public interface ILaunchManager
    {
        LaunchData SignLaunch(Session session, Guid appId);
    }
}
