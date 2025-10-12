using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_AspNetUsers_AppUserId",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_CreatorUserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Parents_AspNetUsers_AppUserId",
                table: "Parents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExams_Answers_AnswerId",
                table: "StudentQuestionAnswerExams");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExams_Exams_ExamId",
                table: "StudentQuestionAnswerExams");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExams_Questions_QuestionId",
                table: "StudentQuestionAnswerExams");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExams_Students_StudentId",
                table: "StudentQuestionAnswerExams");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_AppUserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_AspNetUsers_AppUserId",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_AspNetUsers_UserId",
                table: "UserNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentQuestionAnswerExams",
                table: "StudentQuestionAnswerExams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "Academics");

            migrationBuilder.EnsureSchema(
                name: "Notifications");

            migrationBuilder.RenameTable(
                name: "UserNotifications",
                newName: "UserNotifications",
                newSchema: "Notifications");

            migrationBuilder.RenameTable(
                name: "TeacherSubjectExams",
                newName: "TeacherSubjectExams",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Teachers",
                newName: "Teachers",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Subjects",
                newName: "Subjects",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "StudentSubjectExams",
                newName: "StudentSubjectExams",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Students",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "Questions",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "QuestionExamTeachers",
                newName: "QuestionExamTeachers",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "ParentStudents",
                newName: "ParentStudents",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Parents",
                newName: "Parents",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notifications",
                newSchema: "Notifications");

            migrationBuilder.RenameTable(
                name: "Exams",
                newName: "Exams",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "ClassSubjects",
                newName: "ClassSubjects",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "Classes",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "ClassAssets",
                newName: "ClassAssets",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "ClassAppointments",
                newName: "ClassAppointments",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Answers",
                newName: "Answers",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "Admins",
                newName: "Admins",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "StudentQuestionAnswerExams",
                newName: "StudentQuestionAnswerExam",
                newSchema: "Academics");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AppUsers",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AppUserRoles",
                newSchema: "Identity");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AppRoles",
                newSchema: "Identity");

            migrationBuilder.RenameIndex(
                name: "IX_StudentQuestionAnswerExams_QuestionId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                newName: "IX_StudentQuestionAnswerExam_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentQuestionAnswerExams_ExamId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                newName: "IX_StudentQuestionAnswerExam_ExamId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentQuestionAnswerExams_AnswerId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                newName: "IX_StudentQuestionAnswerExam_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "Identity",
                table: "AppUserRoles",
                newName: "IX_AppUserRoles_RoleId");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                schema: "Identity",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentQuestionAnswerExam",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                columns: new[] { "StudentId", "ExamId", "QuestionId", "AnswerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUsers",
                schema: "Identity",
                table: "AppUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUserRoles",
                schema: "Identity",
                table: "AppUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppRoles",
                schema: "Identity",
                table: "AppRoles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalSchema: "Identity",
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AppUserId",
                schema: "Identity",
                table: "RefreshTokens",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "Identity",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_AppUsers_AppUserId",
                schema: "Identity",
                table: "Admins",
                column: "AppUserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_AppRoles_RoleId",
                schema: "Identity",
                table: "AppUserRoles",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserRoles_AppUsers_UserId",
                schema: "Identity",
                table: "AppUserRoles",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AppRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalSchema: "Identity",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AppUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AppUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AppUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AppUsers_CreatorUserId",
                schema: "Notifications",
                table: "Notifications",
                column: "CreatorUserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Parents_AppUsers_AppUserId",
                schema: "Academics",
                table: "Parents",
                column: "AppUserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExam_Answers_AnswerId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                column: "AnswerId",
                principalSchema: "Academics",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExam_Exams_ExamId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                column: "ExamId",
                principalSchema: "Academics",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExam_Questions_QuestionId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                column: "QuestionId",
                principalSchema: "Academics",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExam_Students_StudentId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam",
                column: "StudentId",
                principalSchema: "Identity",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AppUsers_AppUserId",
                schema: "Identity",
                table: "Students",
                column: "AppUserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_AppUsers_AppUserId",
                schema: "Academics",
                table: "Teachers",
                column: "AppUserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_AppUsers_UserId",
                schema: "Notifications",
                table: "UserNotifications",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_AppUsers_AppUserId",
                schema: "Identity",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_AppRoles_RoleId",
                schema: "Identity",
                table: "AppUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserRoles_AppUsers_UserId",
                schema: "Identity",
                table: "AppUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AppRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AppUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AppUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AppUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AppUsers_CreatorUserId",
                schema: "Notifications",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Parents_AppUsers_AppUserId",
                schema: "Academics",
                table: "Parents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExam_Answers_AnswerId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExam_Exams_ExamId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExam_Questions_QuestionId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentQuestionAnswerExam_Students_StudentId",
                schema: "Academics",
                table: "StudentQuestionAnswerExam");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AppUsers_AppUserId",
                schema: "Identity",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_AppUsers_AppUserId",
                schema: "Academics",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_AppUsers_UserId",
                schema: "Notifications",
                table: "UserNotifications");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "Identity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentQuestionAnswerExam",
                schema: "Academics",
                table: "StudentQuestionAnswerExam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUsers",
                schema: "Identity",
                table: "AppUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUserRoles",
                schema: "Identity",
                table: "AppUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppRoles",
                schema: "Identity",
                table: "AppRoles");

            migrationBuilder.DropColumn(
                name: "ContactInfo",
                schema: "Identity",
                table: "AppUsers");

            migrationBuilder.RenameTable(
                name: "UserNotifications",
                schema: "Notifications",
                newName: "UserNotifications");

            migrationBuilder.RenameTable(
                name: "TeacherSubjectExams",
                schema: "Academics",
                newName: "TeacherSubjectExams");

            migrationBuilder.RenameTable(
                name: "Teachers",
                schema: "Academics",
                newName: "Teachers");

            migrationBuilder.RenameTable(
                name: "Subjects",
                schema: "Academics",
                newName: "Subjects");

            migrationBuilder.RenameTable(
                name: "StudentSubjectExams",
                schema: "Academics",
                newName: "StudentSubjectExams");

            migrationBuilder.RenameTable(
                name: "Students",
                schema: "Identity",
                newName: "Students");

            migrationBuilder.RenameTable(
                name: "Questions",
                schema: "Academics",
                newName: "Questions");

            migrationBuilder.RenameTable(
                name: "QuestionExamTeachers",
                schema: "Academics",
                newName: "QuestionExamTeachers");

            migrationBuilder.RenameTable(
                name: "ParentStudents",
                schema: "Academics",
                newName: "ParentStudents");

            migrationBuilder.RenameTable(
                name: "Parents",
                schema: "Academics",
                newName: "Parents");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "Notifications",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "Exams",
                schema: "Academics",
                newName: "Exams");

            migrationBuilder.RenameTable(
                name: "ClassSubjects",
                schema: "Academics",
                newName: "ClassSubjects");

            migrationBuilder.RenameTable(
                name: "Classes",
                schema: "Academics",
                newName: "Classes");

            migrationBuilder.RenameTable(
                name: "ClassAssets",
                schema: "Academics",
                newName: "ClassAssets");

            migrationBuilder.RenameTable(
                name: "ClassAppointments",
                schema: "Academics",
                newName: "ClassAppointments");

            migrationBuilder.RenameTable(
                name: "Answers",
                schema: "Academics",
                newName: "Answers");

            migrationBuilder.RenameTable(
                name: "Admins",
                schema: "Identity",
                newName: "Admins");

            migrationBuilder.RenameTable(
                name: "StudentQuestionAnswerExam",
                schema: "Academics",
                newName: "StudentQuestionAnswerExams");

            migrationBuilder.RenameTable(
                name: "AppUsers",
                schema: "Identity",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AppUserRoles",
                schema: "Identity",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AppRoles",
                schema: "Identity",
                newName: "AspNetRoles");

            migrationBuilder.RenameIndex(
                name: "IX_StudentQuestionAnswerExam_QuestionId",
                table: "StudentQuestionAnswerExams",
                newName: "IX_StudentQuestionAnswerExams_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentQuestionAnswerExam_ExamId",
                table: "StudentQuestionAnswerExams",
                newName: "IX_StudentQuestionAnswerExams_ExamId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentQuestionAnswerExam_AnswerId",
                table: "StudentQuestionAnswerExams",
                newName: "IX_StudentQuestionAnswerExams_AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUserRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentQuestionAnswerExams",
                table: "StudentQuestionAnswerExams",
                columns: new[] { "StudentId", "ExamId", "QuestionId", "AnswerId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_AspNetUsers_AppUserId",
                table: "Admins",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_CreatorUserId",
                table: "Notifications",
                column: "CreatorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Parents_AspNetUsers_AppUserId",
                table: "Parents",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExams_Answers_AnswerId",
                table: "StudentQuestionAnswerExams",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExams_Exams_ExamId",
                table: "StudentQuestionAnswerExams",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExams_Questions_QuestionId",
                table: "StudentQuestionAnswerExams",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentQuestionAnswerExams_Students_StudentId",
                table: "StudentQuestionAnswerExams",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_AppUserId",
                table: "Students",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_AspNetUsers_AppUserId",
                table: "Teachers",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_AspNetUsers_UserId",
                table: "UserNotifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
