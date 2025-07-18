// Gestion de l'interface utilisateur
document.addEventListener('DOMContentLoaded', () => {
    // Éléments du DOM pour l'authentification
    const authSection = document.getElementById('auth-section');
    const loginForm = document.getElementById('login-form');
    const registerForm = document.getElementById('register-form');
    const showLoginBtn = document.getElementById('show-login');
    const showRegisterBtn = document.getElementById('show-register');
    const loginBtn = document.getElementById('login-btn');
    const registerBtn = document.getElementById('register-btn');
    
    // Éléments du DOM pour l'application
    const appSection = document.getElementById('app-section');
    const logoutBtn = document.getElementById('logout-btn');
    const usernameDisplay = document.getElementById('username-display');
    const totalSessionsDisplay = document.getElementById('total-sessions');
    const totalMinutesDisplay = document.getElementById('total-minutes');
    const sessionsWeekDisplay = document.getElementById('sessions-week');
    const motivationQuoteElement = document.getElementById('motivation-quote');
    
    // Éléments du DOM pour les sessions
    const createSessionBtn = document.getElementById('create-session-btn');
    const activeSessionSection = document.getElementById('active-session');
    const activeSessionNameElement = document.getElementById('active-session-name');
    const activeSessionDescriptionElement = document.getElementById('active-session-description');
    const timerDisplay = document.getElementById('timer-display');
    const endSessionBtn = document.getElementById('end-session-btn');
    const sessionListElement = document.getElementById('session-list');
    
    // Éléments du DOM pour la liste noire
    const addBlacklistBtn = document.getElementById('add-blacklist-btn');
    const blacklistListElement = document.getElementById('blacklist-list');
    
    // Toast pour les notifications
    const toast = document.getElementById('toast');
    const toastMessage = document.getElementById('toast-message');
    
    // Variables pour gérer l'état de l'application
    let activeSession = null;
    let timerInterval = null;
    let secondsRemaining = 0;
    
    // Initialiser l'application
    init();
    
    // Fonction d'initialisation
    async function init() {
        // Vérifier si l'utilisateur est déjà connecté
        if (authService.isAuthenticated()) {
            await showApp();
        } else {
            showAuth();
        }
        
        // Obtenir une citation motivante aléatoire
        try {
            const quote = await statsService.getRandomQuote();
            motivationQuoteElement.textContent = quote;
        } catch (error) {
            console.error('Erreur lors de la récupération de la citation:', error);
        }
    }
    
    // Fonction pour afficher l'interface d'authentification
    function showAuth() {
        authSection.style.display = 'block';
        appSection.style.display = 'none';
        
        // Événements pour basculer entre connexion et inscription
        showLoginBtn.addEventListener('click', (e) => {
            e.preventDefault();
            loginForm.style.display = 'block';
            registerForm.style.display = 'none';
        });
        
        showRegisterBtn.addEventListener('click', (e) => {
            e.preventDefault();
            loginForm.style.display = 'none';
            registerForm.style.display = 'block';
        });
        
        // Événement pour la connexion
        loginBtn.addEventListener('click', async () => {
            const username = document.getElementById('login-username').value;
            const password = document.getElementById('login-password').value;
            
            if (!username || !password) {
                showToast('Veuillez remplir tous les champs', 'error');
                return;
            }
            
            try {
                await authService.login(username, password);
                await showApp();
            } catch (error) {
                showToast(error.message || 'Erreur lors de la connexion', 'error');
            }
        });
        
        // Événement pour l'inscription
        registerBtn.addEventListener('click', async () => {
            const username = document.getElementById('register-username').value;
            const email = document.getElementById('register-email').value;
            const password = document.getElementById('register-password').value;
            const confirmPassword = document.getElementById('register-confirm-password').value;
            
            if (!username || !email || !password || !confirmPassword) {
                showToast('Veuillez remplir tous les champs', 'error');
                return;
            }
            
            if (password !== confirmPassword) {
                showToast('Les mots de passe ne correspondent pas', 'error');
                return;
            }
            
            try {
                await authService.register(username, email, password, confirmPassword);
                await showApp();
            } catch (error) {
                showToast(error.message || "Erreur lors de l'inscription", 'error');
            }
        });
    }
    
    // Fonction pour afficher l'application principale
    async function showApp() {
        authSection.style.display = 'none';
        appSection.style.display = 'block';
        
        // Afficher le nom d'utilisateur
        usernameDisplay.textContent = authService.user.username;
        
        // Charger les statistiques
        await loadStats();
        
        // Charger l'historique des sessions
        await loadSessions();
        
        // Charger les éléments de la liste noire
        await loadBlacklist();
        
        // Événement pour la déconnexion
        logoutBtn.addEventListener('click', () => {
            authService.logout();
            stopTimer();
            showAuth();
        });
        
        // Événement pour créer une session
        createSessionBtn.addEventListener('click', async () => {
            const name = document.getElementById('session-name').value;
            const description = document.getElementById('session-description').value;
            const duration = parseInt(document.getElementById('session-duration').value);
            
            if (!name || isNaN(duration) || duration < 1) {
                showToast('Veuillez remplir correctement les champs', 'error');
                return;
            }
            
            try {
                const session = await sessionService.createSession(name, description, duration);
                await startSession(session);
                showToast('Session créée et démarrée!', 'success');
            } catch (error) {
                showToast(error.message || 'Erreur lors de la création de la session', 'error');
            }
        });
        
        // Événement pour terminer une session
        endSessionBtn.addEventListener('click', async () => {
            if (!activeSession) {
                return;
            }
            
            try {
                const notes = prompt('Ajoutez des notes pour cette session (optionnel):');
                await sessionService.endSession(activeSession.id, notes);
                stopTimer();
                activeSession = null;
                activeSessionSection.style.display = 'none';
                await loadSessions();
                await loadStats();
                showToast('Session terminée avec succès!', 'success');
            } catch (error) {
                showToast(error.message || 'Erreur lors de la fin de la session', 'error');
            }
        });
        
        // Événement pour ajouter un site à la liste noire
        addBlacklistBtn.addEventListener('click', async () => {
            const url = document.getElementById('blacklist-url').value;
            const name = document.getElementById('blacklist-name').value;
            const reason = document.getElementById('blacklist-reason').value;
            
            if (!url) {
                showToast('Veuillez saisir une URL', 'error');
                return;
            }
            
            try {
                await blacklistService.addBlacklistItem(url, name, reason);
                document.getElementById('blacklist-url').value = '';
                document.getElementById('blacklist-name').value = '';
                document.getElementById('blacklist-reason').value = '';
                await loadBlacklist();
                showToast('Site ajouté à la liste noire!', 'success');
            } catch (error) {
                showToast(error.message || "Erreur lors de l'ajout à la liste noire", 'error');
            }
        });
    }
    
    // Charger les statistiques de l'utilisateur
    async function loadStats() {
        try {
            const stats = await statsService.getStats();
            totalSessionsDisplay.textContent = stats.totalSessions;
            totalMinutesDisplay.textContent = stats.totalMinutes;
            sessionsWeekDisplay.textContent = stats.sessionsLastWeek;
        } catch (error) {
            console.error('Erreur lors du chargement des statistiques:', error);
            totalSessionsDisplay.textContent = '0';
            totalMinutesDisplay.textContent = '0';
            sessionsWeekDisplay.textContent = '0';
        }
    }
    
    // Charger l'historique des sessions
    async function loadSessions() {
        try {
            const sessions = await sessionService.getAllSessions();
            sessionListElement.innerHTML = '';
            
            if (sessions.length === 0) {
                sessionListElement.innerHTML = '<p class="empty-message">Aucune session pour le moment</p>';
                return;
            }
            
            sessions.forEach(session => {
                const sessionItem = document.createElement('div');
                sessionItem.classList.add('session-item');
                
                const formattedDate = new Date(session.startTime).toLocaleDateString('fr-FR', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                });
                
                sessionItem.innerHTML = `
                    <div class="session-item-details">
                        <h4>${session.name}</h4>
                        <p>${session.description || 'Pas de description'}</p>
                        <p>Date: ${formattedDate}</p>
                        <p>Durée: ${session.isCompleted ? session.actualDurationMinutes || 0 : session.plannedDurationMinutes} minutes</p>
                        <p>Statut: ${session.isCompleted ? 'Terminée' : 'En attente'}</p>
                    </div>
                    <div class="session-item-actions">
                        ${!session.isCompleted ? 
                            `<button class="btn small start-session" data-id="${session.id}">Démarrer</button>` : ''}
                        <button class="btn small danger delete-session" data-id="${session.id}">Supprimer</button>
                    </div>
                `;
                
                sessionListElement.appendChild(sessionItem);
                
                // Événements pour les boutons de session
                const startBtn = sessionItem.querySelector('.start-session');
                if (startBtn) {
                    startBtn.addEventListener('click', async () => {
                        try {
                            const startedSession = await sessionService.startSession(session.id);
                            await startSession(startedSession);
                            showToast('Session démarrée!', 'success');
                        } catch (error) {
                            showToast(error.message || 'Erreur lors du démarrage de la session', 'error');
                        }
                    });
                }
                
                const deleteBtn = sessionItem.querySelector('.delete-session');
                deleteBtn.addEventListener('click', async () => {
                    if (confirm('Êtes-vous sûr de vouloir supprimer cette session?')) {
                        try {
                            await sessionService.deleteSession(session.id);
                            await loadSessions();
                            await loadStats();
                            showToast('Session supprimée!', 'success');
                        } catch (error) {
                            showToast(error.message || 'Erreur lors de la suppression de la session', 'error');
                        }
                    }
                });
            });
        } catch (error) {
            console.error('Erreur lors du chargement des sessions:', error);
            sessionListElement.innerHTML = '<p class="error-message">Erreur lors du chargement des sessions</p>';
        }
    }
    
    // Charger les éléments de la liste noire
    async function loadBlacklist() {
        try {
            const items = await blacklistService.getAllBlacklistItems();
            blacklistListElement.innerHTML = '';
            
            if (items.length === 0) {
                blacklistListElement.innerHTML = '<p class="empty-message">Aucun site bloqué pour le moment</p>';
                return;
            }
            
            items.forEach(item => {
                const blacklistItem = document.createElement('div');
                blacklistItem.classList.add('blacklist-item');
                
                blacklistItem.innerHTML = `
                    <div class="blacklist-item-details">
                        <h4>${item.name || item.url}</h4>
                        <p><a href="${item.url}" target="_blank">${item.url}</a></p>
                        <p>${item.reason || 'Pas de raison spécifiée'}</p>
                    </div>
                    <div class="blacklist-item-actions">
                        <button class="btn small danger delete-blacklist" data-id="${item.id}">Supprimer</button>
                    </div>
                `;
                
                blacklistListElement.appendChild(blacklistItem);
                
                // Événement pour supprimer de la liste noire
                const deleteBtn = blacklistItem.querySelector('.delete-blacklist');
                deleteBtn.addEventListener('click', async () => {
                    if (confirm('Êtes-vous sûr de vouloir retirer ce site de la liste noire?')) {
                        try {
                            await blacklistService.deleteBlacklistItem(item.id);
                            await loadBlacklist();
                            showToast('Site retiré de la liste noire!', 'success');
                        } catch (error) {
                            showToast(error.message || 'Erreur lors de la suppression de la liste noire', 'error');
                        }
                    }
                });
            });
        } catch (error) {
            console.error('Erreur lors du chargement de la liste noire:', error);
            blacklistListElement.innerHTML = '<p class="error-message">Erreur lors du chargement de la liste noire</p>';
        }
    }
    
    // Démarrer une session active
    async function startSession(session) {
        stopTimer(); // Arrêter le timer précédent si existant
        
        activeSession = session;
        activeSessionSection.style.display = 'block';
        activeSessionNameElement.textContent = session.name;
        activeSessionDescriptionElement.textContent = session.description || 'Pas de description';
        
        // Démarrer le timer
        secondsRemaining = session.plannedDurationMinutes * 60;
        updateTimerDisplay();
        
        timerInterval = setInterval(() => {
            secondsRemaining--;
            updateTimerDisplay();
            
            if (secondsRemaining <= 0) {
                stopTimer();
                alert('Temps écoulé! Votre session est terminée.');
                // Appel automatique pour terminer la session
                (async () => {
                    try {
                        await sessionService.endSession(activeSession.id, 'Session terminée automatiquement après écoulement du temps');
                        activeSession = null;
                        activeSessionSection.style.display = 'none';
                        await loadSessions();
                        await loadStats();
                    } catch (error) {
                        console.error('Erreur lors de la fin automatique de la session:', error);
                    }
                })();
            }
        }, 1000);
    }
    
    // Arrêter le timer
    function stopTimer() {
        clearInterval(timerInterval);
        timerInterval = null;
    }
    
    // Mettre à jour l'affichage du timer
    function updateTimerDisplay() {
        const minutes = Math.floor(secondsRemaining / 60);
        const seconds = secondsRemaining % 60;
        timerDisplay.textContent = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    }
    
    // Afficher un toast de notification
    function showToast(message, type = 'info') {
        toastMessage.textContent = message;
        toast.className = 'toast';
        toast.classList.add(type);
        toast.classList.add('visible');
        
        setTimeout(() => {
            toast.classList.remove('visible');
        }, 3000);
    }
});
