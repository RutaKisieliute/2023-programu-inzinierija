﻿// <auto-generated />
using System;
using AALKisAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AALKisAPI.Migrations
{
    [DbContext(typeof(NoteDB))]
    partial class NoteDBModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AALKisAPI.Models.Folder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.Property<int?>("UserId")
                        .HasColumnType("int(11)")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("folders", (string)null);
                });

            modelBuilder.Entity("AALKisAPI.Models.Keyword", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name")
                        .HasDefaultValueSql("''");

                    b.Property<int>("NoteId")
                        .HasColumnType("int(11)")
                        .HasColumnName("note_id");

                    b.HasKey("Name", "NoteId")
                        .HasName("PRIMARY");

                    b.HasIndex("NoteId");

                    b.ToTable("keywords", (string)null);
                });

            modelBuilder.Entity("AALKisAPI.Models.Note", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int(11)")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<sbyte?>("Flags")
                        .HasColumnType("tinyint(4)")
                        .HasColumnName("flags");

                    b.Property<int?>("FolderId")
                        .HasColumnType("int(11)")
                        .HasColumnName("folder_id");

                    b.Property<DateTime>("Modified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp")
                        .HasColumnName("modified")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Title")
                        .HasColumnType("text")
                        .HasColumnName("title");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "FolderId" }, "folder_id");

                    b.ToTable("notes", (string)null);
                });

            modelBuilder.Entity("AALKisAPI.Models.Tag", b =>
                {
                    b.Property<int>("NoteId")
                        .HasColumnType("int(11)")
                        .HasColumnName("note_id");

                    b.Property<string>("Tag1")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tag")
                        .HasDefaultValueSql("''");

                    b.HasKey("NoteId", "Tag1")
                        .HasName("PRIMARY");

                    b.ToTable("tags", (string)null);
                });

            modelBuilder.Entity("AALKisAPI.Models.Keyword", b =>
                {
                    b.HasOne("AALKisAPI.Models.Note", "Note")
                        .WithMany("Keywords")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("keywords_ibfk_1");

                    b.Navigation("Note");
                });

            modelBuilder.Entity("AALKisAPI.Models.Note", b =>
                {
                    b.HasOne("AALKisAPI.Models.Folder", "Folder")
                        .WithMany("Notes")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("notes_ibfk_1");

                    b.Navigation("Folder");
                });

            modelBuilder.Entity("AALKisAPI.Models.Tag", b =>
                {
                    b.HasOne("AALKisAPI.Models.Note", "Note")
                        .WithMany("Tags")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("tags_ibfk_1");

                    b.Navigation("Note");
                });

            modelBuilder.Entity("AALKisAPI.Models.Folder", b =>
                {
                    b.Navigation("Notes");
                });

            modelBuilder.Entity("AALKisAPI.Models.Note", b =>
                {
                    b.Navigation("Keywords");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
