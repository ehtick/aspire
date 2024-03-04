// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

var builder = DistributedApplication.CreateBuilder(args);
builder.AddAzureProvisioning();

var sku = builder.AddParameter("storagesku");
var locationOverride = builder.AddParameter("locationOverride");

var storage = builder.AddAzureConstructStorage("storage", (_, account) =>
{
    account.AssignParameter(sa => sa.Sku.Name, sku);
    account.AssignParameter(sa => sa.Location, locationOverride);
});

var blobs = storage.AddBlobs("blobs");

var sqldb = builder.AddSqlServer("sql").AsAzureSqlDatabaseConstruct().AddDatabase("sqldb");

builder.AddProject<Projects.CdkSample_ApiService>("api")
       .WithReference(blobs)
       .WithReference(sqldb);

// This project is only added in playground projects to support development/debugging
// of the dashboard. It is not required in end developer code. Comment out this code
// to test end developer dashboard launch experience. Refer to Directory.Build.props
// for the path to the dashboard binary (defaults to the Aspire.Dashboard bin output
// in the artifacts dir).
builder.AddProject<Projects.Aspire_Dashboard>(KnownResourceNames.AspireDashboard);

builder.Build().Run();