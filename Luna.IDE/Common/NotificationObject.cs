﻿using System.ComponentModel;
using System.Linq.Expressions;

namespace Luna.IDE.Common;

public abstract class NotificationObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged(Expression<Func<object>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
        }
        else if (propertyExpression.Body is UnaryExpression unaryExpression)
        {
            var innerMemberExpression = (MemberExpression)unaryExpression.Operand;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(innerMemberExpression.Member.Name));
        }
    }
}
