using System;
using AALKisAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AALKisAPI.Data;

public partial class NoteDB : DbContext
{
    private readonly string _dbConnection;

    private readonly MySqlServerVersion _serverVersion;

    private readonly SaveChangesInterceptor _saveChangesInterceptor = new TestLoggerInterceptor();
    
    public NoteDB(ConnectionString connectionString, SaveChangesInterceptor? saveChangesInterceptor = null)
    {
        _dbConnection = connectionString.Value!;
        _serverVersion = new MySqlServerVersion(new Version(5, 5, 62));
        if(saveChangesInterceptor != null) _saveChangesInterceptor = saveChangesInterceptor;
        //new LoggerInterceptor("./Logs");
    }

    public NoteDB(DbContextOptions<NoteDB> options, ConnectionString connectionString, SaveChangesInterceptor? saveChangesInterceptor = null)
        : base(options)
    {
        _dbConnection = connectionString.Value!;
        _serverVersion = new MySqlServerVersion(new Version(5, 5, 62));
        if(saveChangesInterceptor != null) _saveChangesInterceptor = saveChangesInterceptor;
        //_saveChangesInterceptor = new LoggerInterceptor("./Logs");
    }

    public virtual DbSet<Folder> Folders { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<Keyword> Keywords { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql(_dbConnection, _serverVersion).AddInterceptors(_saveChangesInterceptor);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Folder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("folders");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Title)
                .HasColumnType("text")
                .HasColumnName("title");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("notes");

            entity.HasIndex(e => e.FolderId, "folder_id");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.Flags)
                .HasColumnType("tinyint(4)")
                .HasColumnName("flags");
            entity.Property(e => e.FolderId)
                .HasColumnType("int(11)")
                .HasColumnName("folder_id");
            entity.Property(e => e.Modified)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("modified");
            entity.Property(e => e.Title)
                .HasColumnType("text")
                .HasColumnName("title");

            entity.HasOne(d => d.Folder).WithMany(p => p.Notes)
                .HasForeignKey(d => d.FolderId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("notes_ibfk_1");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => new { e.NoteId, e.Tag1 }).HasName("PRIMARY");

            entity.ToTable("tags");

            entity.Property(e => e.NoteId)
                .HasColumnType("int(11)")
                .HasColumnName("note_id");
            entity.Property(e => e.Tag1)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("tag");

            entity.HasOne(d => d.Note).WithMany(p => p.Tags)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("tags_ibfk_1");
        });

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.HasKey(e => new { e.Name, e.NoteId }).HasName("PRIMARY");

            entity.ToTable("keywords");

            entity.Property(e => e.NoteId)
                .HasColumnType("int(11)")
                .HasColumnName("note_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("name");

            entity.HasOne(d => d.Note).WithMany(p => p.Keywords)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("keywords_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
