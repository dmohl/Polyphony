module SettingsProvider

open System.Configuration

type ISettingsProvider = interface   
    abstract GetApplicationSetting : itemKey:string -> string
end

type SettingsProvider() = class
    interface ISettingsProvider with
        member x.GetApplicationSetting itemKey =
            ConfigurationManager.AppSettings.Item(itemKey)
end
