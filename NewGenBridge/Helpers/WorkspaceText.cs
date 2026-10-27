using Microsoft.AspNetCore.Mvc.Rendering;

namespace NewGenBridge.Helpers;

// UI copy follows the existing language cookie without changing the imported database.
public static class WorkspaceText
{
    private static readonly Dictionary<string, string[]> Copy = new()
    {
        ["change_week"] = ["Change week", "Перейти к другой неделе", "Change week", "Change week", "Change week", "Change week"],
        ["group_settings"] = ["Group and lesson settings", "Настройки группы и урока", "Group and lesson settings", "Group and lesson settings", "Group and lesson settings", "Group and lesson settings"],
        ["move_group"] = ["Move to group", "Перевести в группу", "Move to group", "Move to group", "Move to group", "Move to group"],
        ["no_group"] = ["No group", "Без группы", "No group", "No group", "No group", "No group"],
        ["found_students"] = ["Students found", "Найдено учеников", "Students found", "Students found", "Students found", "Students found"],
        ["all_groups"] = ["All groups", "Все группы", "All groups", "All groups", "All groups", "All groups"],
        ["all_branches"] = ["All branches", "Все филиалы", "All branches", "All branches", "All branches", "All branches"],
        ["continue"] = ["Continue", "Продолжить", "Continue", "Continue", "Continue", "Continue"],
        ["choose_group"] = ["Choose a group to add a student", "Выберите группу для нового ученика", "Choose a group to add a student", "Choose a group to add a student", "Choose a group to add a student", "Choose a group to add a student"],
        ["requests"] = ["Registration requests", "Заявки на регистрацию", "Registration requests", "Registration requests", "Registration requests", "Registration requests"],
        ["saved"] = ["Saved", "Сохранено", "Saved", "Saved", "Saved", "Saved"],
        ["save_error"] = ["Could not save. Try again.", "Не удалось сохранить. Попробуйте ещё раз.", "Could not save. Try again.", "Could not save. Try again.", "Could not save. Try again.", "Could not save. Try again."],
        ["attendance_hint"] = ["Select a cell: not marked, present, absent. Then save.", "Нажмите на клетку: не отмечено → присутствовал → отсутствовал. Затем сохраните.", "Select a cell: not marked, present, absent. Then save.", "Select a cell: not marked, present, absent. Then save.", "Select a cell: not marked, present, absent. Then save.", "Select a cell: not marked, present, absent. Then save."],
        ["unmarked"] = ["Not marked", "Не отмечено", "Not marked", "Not marked", "Not marked", "Not marked"],
        ["present"] = ["Present", "Присутствовал", "Present", "Present", "Present", "Present"],
        ["absent"] = ["Absent", "Отсутствовал", "Absent", "Absent", "Absent", "Absent"],
        ["locked_hint"] = ["Access is not open. Contact your manager.", "Доступ не открыт. Обратитесь к менеджеру.", "Access is not open. Contact your manager.", "Access is not open. Contact your manager.", "Access is not open. Contact your manager.", "Access is not open. Contact your manager."],
        ["confirm_action"] = ["Confirm action", "Подтвердите действие", "Confirm action", "Confirm action", "Confirm action", "Confirm action"],
        ["unsaved"] = ["There are unsaved changes. Leave?", "Есть несохранённые изменения. Выйти?", "There are unsaved changes. Leave?", "There are unsaved changes. Leave?", "There are unsaved changes. Leave?", "There are unsaved changes. Leave?"],
        ["all_statuses"] = ["All statuses", "Все статусы", "All statuses", "All statuses", "All statuses", "All statuses"],
        ["results_error"] = ["Could not load results. Try again.", "Не удалось загрузить результаты. Попробуйте ещё раз.", "Could not load results. Try again.", "Could not load results. Try again.", "Could not load results. Try again.", "Could not load results. Try again."],
        ["workspace"] = ["Workspace", "Рабочий стол", "Espace de travail", "Area di lavoro", "Çalışma alanı", "Arbeitsbereich"],
        ["manager_home"] = ["Your branch, at a glance", "Ваш филиал", "Votre centre en un coup d’œil", "Il tuo centro a colpo d’occhio", "Şubenize genel bakış", "Ihre Filiale im Überblick"],
        ["supervisor_home"] = ["Your network, at a glance", "Обзор сети", "Votre réseau en un coup d’œil", "La tua rete a colpo d’occhio", "Ağınıza genel bakış", "Ihr Netzwerk im Überblick"],
        ["admin_intro"] = ["Courses, branches and access — everything you manage in one place.", "Курсы, филиалы и доступы — всё для управления в одном месте.", "Cours, centres et accès : tout pour votre gestion.", "Corsi, centri e accessi: tutto per la gestione.", "Kurslar, şubeler ve erişim yönetimi tek yerde.", "Kurse, Filialen und Zugänge an einem Ort verwalten."],
        ["manager_intro"] = ["Start with your students, organise groups and keep documents close.", "Работайте с учениками, управляйте группами и находите нужные документы.", "Gérez les élèves, les groupes et leurs documents.", "Gestisci studenti, gruppi e documenti.", "Öğrencileri, grupları ve belgeleri yönetin.", "Schüler, Gruppen und Dokumente verwalten."],
        ["supervisor_intro"] = ["Navigate your branches, review finances and manage team access.", "Переходите к филиалам, проверяйте финансы и управляйте доступом команды.", "Consultez vos centres, les finances et les accès de l’équipe.", "Consulta i centri, le finanze e gli accessi del team.", "Şubeleri, finansları ve ekip erişimini yönetin.", "Filialen, Finanzen und Teamzugänge im Blick behalten."],
        ["learning"] = ["Learning materials", "Учебные материалы", "Supports de cours", "Materiale didattico", "Ders materyalleri", "Lernmaterialien"],
        ["course_hint"] = ["Open a course to work with its lessons and projects.", "Откройте курс, чтобы перейти к урокам и проектам.", "Ouvrez un cours pour accéder aux leçons et projets.", "Apri un corso per accedere a lezioni e progetti.", "Derslere ve projelere erişmek için bir kurs açın.", "Öffnen Sie einen Kurs für Lektionen und Projekte."],
        ["branch_hint"] = ["Groups, schedules and attendance in your branches.", "Группы, расписание и посещаемость в ваших филиалах.", "Groupes, horaires et présences de vos centres.", "Gruppi, orari e presenze dei tuoi centri.", "Şubelerinizdeki gruplar, programlar ve yoklama.", "Gruppen, Stundenpläne und Anwesenheit Ihrer Filialen."],
        ["students_hint"] = ["Find a student, open their details or update their group.", "Найдите ученика, откройте его карточку или измените группу.", "Trouvez un élève, consultez sa fiche ou changez son groupe.", "Trova uno studente, apri la scheda o cambia gruppo.", "Öğrenciyi bulun, bilgilerini açın veya grubunu değiştirin.", "Schüler finden, Details öffnen oder die Gruppe ändern."],
        ["schedule"] = ["My schedule", "Моё расписание", "Mon planning", "Il mio programma", "Programım", "Mein Stundenplan"],
        ["schedule_hint"] = ["Choose a group to open its attendance register.", "Выберите группу, чтобы открыть журнал посещаемости.", "Choisissez un groupe pour ouvrir le registre de présence.", "Scegli un gruppo per aprire il registro presenze.", "Yoklama kaydını açmak için bir grup seçin.", "Wählen Sie eine Gruppe für die Anwesenheitsliste."],
        ["all_days"] = ["All days", "Все дни", "Tous les jours", "Tutti i giorni", "Tüm günler", "Alle Tage"],
        ["today"] = ["Today", "Сегодня", "Aujourd’hui", "Oggi", "Bugün", "Heute"],
        ["no_groups"] = ["No groups scheduled", "Занятий пока нет", "Aucun groupe prévu", "Nessun gruppo in programma", "Planlanmış grup yok", "Keine Gruppen geplant"],
        ["no_groups_hint"] = ["Try another day or contact your manager about your schedule.", "Выберите другой день или уточните расписание у менеджера.", "Choisissez un autre jour ou contactez votre responsable.", "Scegli un altro giorno o contatta il responsabile.", "Başka bir gün seçin veya yöneticinize danışın.", "Wählen Sie einen anderen Tag oder fragen Sie Ihre Leitung."],
        ["all_lessons"] = ["All my lessons", "Все мои уроки", "Toutes mes leçons", "Tutte le mie lezioni", "Tüm derslerim", "Alle meine Lektionen"],
        ["find"] = ["Find", "Найти", "Rechercher", "Cerca", "Bul", "Suchen"],
        ["reset"] = ["Reset", "Сбросить", "Réinitialiser", "Reimposta", "Sıfırla", "Zurücksetzen"],
        ["loading"] = ["Loading…", "Загрузка…", "Chargement…", "Caricamento…", "Yükleniyor…", "Wird geladen…"],
        ["search_error"] = ["Could not load students. Try again.", "Не удалось загрузить учеников. Попробуйте ещё раз.", "Chargement impossible. Réessayez.", "Impossibile caricare gli studenti. Riprova.", "Öğrenciler yüklenemedi. Tekrar deneyin.", "Schüler konnten nicht geladen werden. Bitte erneut versuchen."],
        ["Admin"] = ["Administrator", "Администратор", "Administrateur", "Amministratore", "Yönetici", "Administrator"],
        ["Manager"] = ["Manager", "Менеджер", "Responsable", "Responsabile", "Şube yöneticisi", "Filialleitung"],
        ["Supervisor"] = ["Supervisor", "Супервайзер", "Superviseur", "Supervisore", "Süpervizör", "Supervisor"],
        ["Mentor"] = ["Mentor", "Наставник", "Mentor", "Mentore", "Eğitmen", "Mentor"],
        ["Student"] = ["Student", "Ученик", "Élève", "Studente", "Öğrenci", "Schüler"]
    };

    public static string W(this IHtmlHelper html, string key)
    {
        var language = html.ViewContext.HttpContext.Request.Cookies["lang"];
        int index = language switch { "ru" => 1, "fr" => 2, "it" => 3, "tr" => 4, "de" => 5, _ => 0 };
        return Copy.TryGetValue(key, out var values) ? values[index] : key;
    }
}
