namespace DevExpressGridInconsistencyDemo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class QueryStatement : IEnumerable<String>
    {
        public QueryStatement()
        {
            this.Statements = new List<String>();
            this.Parameters = new Dictionary<String, Object>();
        }

        public QueryStatement(String statement)
            : this()
        {
            this.Add(statement);
        }

        public QueryStatement(IEnumerable<String> statementList, IDictionary<String, Object> parameters)
            : this()
        {
            this.AddRange(statementList);
            this.AddParameter(parameters);
        }

        public QueryStatement(IEnumerable<String> statementList)
            : this()
        {
            this.AddRange(statementList);
        }

        private List<String> Statements { get; }

        public Dictionary<String, Object> Parameters { get; }

        public String Statement
        {
            get
            {
                return String.Join(Environment.NewLine, this.Statements);
            }
        }

        public void Add(String statement)
        {
            this.Statements.Add(statement);
        }

        public void AddRange(IEnumerable<String> statements)
        {
            this.Statements.AddRange(statements);
        }

        public void AddParameter(IDictionary<String, Object> parameters)
        {
            foreach (var parameter in parameters)
            {
                this.Parameters.Add(parameter.Key, parameter.Value);
            }
        }

        public void AddParameter(String name, Object value)
        {
            this.Parameters.Add(name, value);
        }
        public override String ToString()
        {
            return Statement;
        }

        public IEnumerator<String> GetEnumerator()
        {
            return Statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}