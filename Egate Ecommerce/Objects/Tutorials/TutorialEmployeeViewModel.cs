using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tutorials.Model;

namespace Egate_Ecommerce.Objects.Tutorials
{
    public class TutorialEmployeeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; } //database reference
        public string EmployeeName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int VideoCount { get; set; }

        public TutorialEmployeeViewModel()
        { }

        public TutorialEmployeeViewModel(employee entity)
        {
            if (entity == null) return;
            this.Id = (int)entity.Id;
            this.EmployeeName = entity.EmployeeName;
            this.Description = entity.Description;
            this.IsActive = entity.IsActive.ToBool();
        }

        public override bool Equals(object obj)
        {
            if (obj is TutorialEmployeeViewModel)
            {
                var o = obj as TutorialEmployeeViewModel;
                return this.Id == o.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
