using EfModelMigrations.Infrastructure.CodeModel;
using EfModelMigrations.Transformations;
using EfModelMigrations.Transformations.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfModelMigrations
{
    //TODO: refaktorovat fluent api pro associace nebo odstranit

    //public abstract class ModelTransformationBuilderBase : IFluentInterface
    //{
    //    protected ModelMigration migration;

    //    public ModelTransformationBuilderBase(ModelMigration migration)
    //    {
    //        this.migration = migration;
    //    }
    //}

    //public class AssociationBuilder : ModelTransformationBuilderBase
    //{
    //    public AssociationBuilder(ModelMigration migration)
    //        : base(migration)
    //    {
    //    }
    //    public AssociationBuilderOneTo One(string className)
    //    {
    //        return new AssociationBuilderOneTo(migration, new AssociationMemberInfo(className, null));
    //    }

    //    public AssociationBuilderOneTo2 One(string className, NavigationPropertyCodeModel navigationProperty)
    //    {
    //        return new AssociationBuilderOneTo2(migration, new AssociationMemberInfo(className, navigationProperty));
    //    }


    //    public AssociationBuilderManyTo Many(string className)
    //    {
    //        return new AssociationBuilderManyTo(migration, new AssociationMemberInfo(className, null));
    //    }

    //    public AssociationBuilderManyTo2 Many(string className, NavigationPropertyCodeModel navigationProperty)
    //    {
    //        return new AssociationBuilderManyTo2(migration, new AssociationMemberInfo(className, navigationProperty));
    //    }
    //}

    //public class AssociationBuilderManyTo : ModelTransformationBuilderBase
    //{
    //    protected AssociationMemberInfo principal;
    //    public AssociationBuilderManyTo(ModelMigration migration, AssociationMemberInfo principal)
    //        : base(migration)
    //    {
    //        this.principal = principal;
    //    }

    //    public JoinTableBuilder ToMany(string className, NavigationPropertyCodeModel navigationProperty)
    //    {
    //        return new JoinTableBuilder(migration, principal, new AssociationMemberInfo(className, navigationProperty));
    //    }
    //}

    //public class AssociationBuilderManyTo2 : AssociationBuilderManyTo
    //{
    //    public AssociationBuilderManyTo2(ModelMigration migration, AssociationMemberInfo principal)
    //        : base(migration, principal)
    //    {
    //    }

    //    public JoinTableBuilder ToMany(string className)
    //    {
    //        return new JoinTableBuilder(migration, principal, new AssociationMemberInfo(className, null));
    //    }
    //}

    //public class AssociationBuilderOneTo : ModelTransformationBuilderBase
    //{
    //    protected AssociationMemberInfo principal;
    //    public AssociationBuilderOneTo(ModelMigration migration, AssociationMemberInfo principal)
    //        : base(migration)
    //    {
    //        this.principal = principal;
    //    }

    //    public AssociationBuilderOneToOnePkFinisher ToOne(string className, NavigationPropertyCodeModel navigationProperty)
    //    {
    //        return new AssociationBuilderOneToOnePkFinisher(migration, principal, new AssociationMemberInfo(className, navigationProperty));
    //    }

    //    public AssociationBuilderOneToOneFkFinisher ToOneWithExplicitForeignKey(string className, NavigationPropertyCodeModel navigationProperty, string[] foreignKeyColumns)
    //    {
    //        return new AssociationBuilderOneToOneFkFinisher(migration, principal, new AssociationMemberInfo(className, navigationProperty), foreignKeyColumns);
    //    }


    //    public AssociationBuilderOneToManyFinisher ToMany(string className, NavigationPropertyCodeModel navigationProperty, string[] foreignKeyColumns)
    //    {
    //        return new AssociationBuilderOneToManyFinisher(migration, principal, new AssociationMemberInfo(className, navigationProperty), foreignKeyColumns);
    //    }

    //    public AssociationBuilderOneToManyFinisher ToManyWithForeignKeyInClass(string className, NavigationPropertyCodeModel navigationProperty, ScalarPropertyCodeModel[] foreignKeyProperties)
    //    {
    //        return new AssociationBuilderOneToManyFinisher(migration, principal, new AssociationMemberInfo(className, navigationProperty), foreignKeyProperties);
    //    }
    //}

    //public class AssociationBuilderOneTo2 : AssociationBuilderOneTo
    //{
    //    public AssociationBuilderOneTo2(ModelMigration migration, AssociationMemberInfo principal)
    //        : base(migration, principal)
    //    {
    //    }

    //    public AssociationBuilderOneToOnePkFinisher ToOne(string className)
    //    {
    //        return new AssociationBuilderOneToOnePkFinisher(migration, principal, new AssociationMemberInfo(className, null));
    //    }

    //    public AssociationBuilderOneToOneFkFinisher ToOneWithExplicitForeignKey(string className, string[] foreignKeyColumns)
    //    {
    //        return new AssociationBuilderOneToOneFkFinisher(migration, principal, new AssociationMemberInfo(className, null), foreignKeyColumns);
    //    }

    //    public AssociationBuilderOneToManyFinisher ToMany(string className, string[] foreignKeyColumns)
    //    {
    //        return new AssociationBuilderOneToManyFinisher(migration, principal, new AssociationMemberInfo(className, null), foreignKeyColumns);
    //    }

    //    public AssociationBuilderOneToManyFinisher ToManyWithForeignKeyInClass(string className, ScalarPropertyCodeModel[] foreignKeyProperties)
    //    {
    //        return new AssociationBuilderOneToManyFinisher(migration, principal, new AssociationMemberInfo(className, null), foreignKeyProperties);
    //    }
    //}


    //public abstract class AssociationBuilderFinisher : ModelTransformationBuilderBase
    //{
    //    protected AssociationMemberInfo principal;
    //    protected AssociationMemberInfo dependent;

    //    protected bool willCascadeOnDelete;

    //    public AssociationBuilderFinisher(ModelMigration migration, AssociationMemberInfo principal, AssociationMemberInfo dependent)
    //        :base(migration)
    //    {
    //        this.principal = principal;
    //        this.dependent = dependent;

    //        this.willCascadeOnDelete = true;
    //    }
    //}

    //public class AssociationBuilderOneToOnePkFinisher : AssociationBuilderFinisher
    //{
    //    private bool bothEndRequired;

    //    public AssociationBuilderOneToOnePkFinisher(ModelMigration migration, AssociationMemberInfo principal, AssociationMemberInfo dependent)
    //        : base(migration, principal, dependent)
    //    {
    //        this.bothEndRequired = false;
    //    }

    //    public AssociationBuilderOneToOnePkFinisher CascadeOnDelete(bool value)
    //    {
    //        this.willCascadeOnDelete = value;
    //        return this;
    //    }

    //    public AssociationBuilderOneToOnePkFinisher BothEndsAreRequired()
    //    {
    //        this.bothEndRequired = true;
    //        return this;
    //    }

    //    public void Add()
    //    {
    //        migration.AddTransformation(new AddOneToOnePrimaryKeyAssociationTransformation(principal, dependent, bothEndRequired, willCascadeOnDelete));
    //    }
    //}

    //public class AssociationBuilderOneToOneFkFinisher : AssociationBuilderFinisher
    //{
    //    private string[] foreignKeyColumnNames;
    //    private OneToOneAssociationType type;

    //    public AssociationBuilderOneToOneFkFinisher(ModelMigration migration, AssociationMemberInfo principal, AssociationMemberInfo dependent, string[] foreignKeyColumns)
    //        : base(migration, principal, dependent)
    //    {
    //        this.foreignKeyColumnNames = foreignKeyColumns;
    //        this.type = OneToOneAssociationType.DependentRequired;
    //    }

    //    public AssociationBuilderOneToOneFkFinisher CascadeOnDelete(bool value)
    //    {
    //        this.willCascadeOnDelete = value;
    //        return this;
    //    }

    //    public AssociationBuilderOneToOneFkFinisher BothEndsAreRequired()
    //    {
    //        this.type = OneToOneAssociationType.BothEndsRequired;
    //        return this;
    //    }

    //    public AssociationBuilderOneToOneFkFinisher BothEndsAreOptional()
    //    {
    //        this.type = OneToOneAssociationType.BothEndsOptional;
    //        return this;
    //    }


    //    public void Add()
    //    {
    //        migration.AddTransformation(new AddOneToOneForeignKeyAssociationTransformation(principal, dependent, type, foreignKeyColumnNames, willCascadeOnDelete));
    //    }
    //}
    
    //public class AssociationBuilderOneToManyFinisher : AssociationBuilderFinisher
    //{
    //    private string[] foreignKeyColumnNames;
    //    private ScalarPropertyCodeModel[] foreignKeyProperties;
    //    private bool isDependentRequired;

    //    public AssociationBuilderOneToManyFinisher(ModelMigration migration, AssociationMemberInfo principal, AssociationMemberInfo dependent, string[] foreignKeyColumns)
    //        : base(migration, principal, dependent)
    //    {
    //        this.foreignKeyColumnNames = foreignKeyColumns;
    //        this.foreignKeyProperties = null;
    //        this.isDependentRequired = false;
    //    }

    //    public AssociationBuilderOneToManyFinisher(ModelMigration migration, AssociationMemberInfo principal, AssociationMemberInfo dependent, ScalarPropertyCodeModel[] foreignKeyProperties)
    //        : base(migration, principal, dependent)
    //    {
    //        this.foreignKeyProperties = foreignKeyProperties;
    //        this.foreignKeyColumnNames = null;
    //    }

    //    public AssociationBuilderOneToManyFinisher CascadeOnDelete(bool value)
    //    {
    //        this.willCascadeOnDelete = value;
    //        return this;
    //    }

    //    public AssociationBuilderOneToManyFinisher IsRequired()
    //    {
    //        this.isDependentRequired = true;
    //        return this;
    //    }
        
    //    public void Add()
    //    {
    //        if(foreignKeyColumnNames == null)
    //        {
    //            migration.AddTransformation(new AddOneToManyAssociationTransformation(principal, dependent, foreignKeyProperties, isDependentRequired, willCascadeOnDelete));
    //        }
    //        else
    //        {
    //            migration.AddTransformation(new AddOneToManyAssociationTransformation(principal, dependent, foreignKeyColumnNames, isDependentRequired, willCascadeOnDelete));
    //        }
            
    //    }
    //}


    //public class JoinTableBuilder : ModelTransformationBuilderBase
    //{
    //    private AssociationMemberInfo principal;
    //    private AssociationMemberInfo dependent;

    //    public JoinTableBuilder(ModelMigration migration, AssociationMemberInfo principal, AssociationMemberInfo dependent)
    //        :base(migration)
    //    {
    //        this.principal = principal;
    //        this.dependent = dependent;
    //    }


    //    public AssociationBuilderManyToManyFinisher WithJoinTable(string tableName, string[] leftForeighKeys, string[] rightForeignKeys)
    //    {
    //        var joinTable = new ManyToManyJoinTable(tableName, leftForeighKeys, rightForeignKeys);
    //        return new AssociationBuilderManyToManyFinisher(migration, principal, dependent, joinTable);
    //    }
    //}

    

    //public class AssociationBuilderManyToManyFinisher : ModelTransformationBuilderBase
    //{
    //    private AssociationMemberInfo principal;
    //    private AssociationMemberInfo dependent;

    //    private ManyToManyJoinTable joinTable;


    //    public AssociationBuilderManyToManyFinisher(ModelMigration migration, AssociationMemberInfo principal, AssociationMemberInfo dependent, ManyToManyJoinTable joinTable)
    //        :base(migration)
    //    {
    //        this.principal = principal;
    //        this.dependent = dependent;
    //        this.joinTable = joinTable;
    //    }


    //    public void Add()
    //    {
    //        migration.AddTransformation(new AddManyToManyAssociationTransformation(principal, dependent, joinTable));
    //    }
    //}
}
