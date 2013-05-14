using System;
using System.Globalization;
using System.Windows.Data;
using Zander.Domain.Entities;
using Zander.Modules.ServerBrowser.Models;
using Zander.Presentation.WPF.Zander.Infrastructure.Base;

namespace Zander.Modules.ServerBrowser.Converters {
    public class ServerConverter : IValueConverter {
        private readonly IEntityMapper<Server, ServerModel> mapper;

        public ServerConverter() {
            this.mapper = new ServerEntityMapper();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var server = value as Server;
            if(server == null) {
                throw new ArgumentException("value must be of type Server");
            }

            return this.mapper.ModelFromEntity(server);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var model = value as ServerModel;
            if(model == null) {
                throw new ArgumentException("value must be of type ServerModel");
            }

            return this.mapper.EntityFromModel(model);
        }
    }
}
