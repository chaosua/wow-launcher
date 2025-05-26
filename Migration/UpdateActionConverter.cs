using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace wow_launcher_cs.Migration;

public class UpdateActionConverter : JavaScriptConverter
{
    public override IEnumerable<Type> SupportedTypes => [typeof(UpdateAction)];

    public override object Deserialize(IDictionary<string, object> dict, Type type, JavaScriptSerializer serializer)
    {
        if (dict == null)
            return null;

        return new UpdateAction
        {
            ActionType = dict.TryGetValue("action_type", out var actionType)
                ? Convert.ToUInt32(actionType)
                : throw new InvalidOperationException("Missing action_type"),

            LocalFilePath = dict.TryGetValue("local_file_path", out var localFilePath)
                ? localFilePath?.ToString()
                : throw new InvalidOperationException("Missing local_file_path"),

            DownloadFilePath = dict.TryGetValue("download_file_path", out var downloadFilePath)
                ? downloadFilePath?.ToString()
                : throw new InvalidOperationException("Missing download_file_path"),

            Hash = dict.TryGetValue("hash", out var hash)
                ? hash?.ToString()
                : null
        };
    }

    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
    {
        if (obj is not UpdateAction action)
            return [];

        return new Dictionary<string, object>
        {
            ["action_type"] = action.ActionType,
            ["local_file_path"] = action.LocalFilePath,
            ["download_file_path"] = action.DownloadFilePath,
            ["hash"] = action.Hash
        };
    }
}