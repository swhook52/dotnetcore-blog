using System;

namespace Domain.Exceptions
{
    public abstract class NotFoundException : Exception
    {
        private readonly string _entity;
        private readonly string _field;
        private readonly object _value;

        public override string Message => $"{_entity} with '{_field}' of '{_value}' not found.";

        protected NotFoundException(string entity, string field, object value)
        {
            _entity = entity;
            _field = field;
            _value = value;
        }
    }
}