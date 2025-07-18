// Configuration de l'API
const API_BASE_URL = 'http://localhost:5114/api';
const API_ENDPOINTS = {
    register: `${API_BASE_URL}/auth/register`,
    login: `${API_BASE_URL}/auth/login`,
    refreshToken: `${API_BASE_URL}/auth/refresh-token`,
    me: `${API_BASE_URL}/auth/me`,
    sessions: `${API_BASE_URL}/session`,
    blacklist: `${API_BASE_URL}/blacklist`,
    quote: `${API_BASE_URL}/motivation/quote`,
    message: `${API_BASE_URL}/motivation/message`,
    stats: `${API_BASE_URL}/stats`
};

// Gestion de l'authentification
class AuthService {
    constructor() {
        this.token = localStorage.getItem('token');
        this.refreshToken = localStorage.getItem('refreshToken');
        this.user = JSON.parse(localStorage.getItem('user') || 'null');
        this.tokenExpiration = localStorage.getItem('tokenExpiration');
    }

    // Vérifier si l'utilisateur est connecté
    isAuthenticated() {
        if (!this.token) {
            return false;
        }
        
        // Vérifier si le token a expiré
        if (this.tokenExpiration && new Date() > new Date(this.tokenExpiration)) {
            return this.refreshTokenFunc();
        }
        
        return true;
    }

    // Enregistrer un nouvel utilisateur
    async register(username, email, password, confirmPassword) {
        try {
            const response = await fetch(API_ENDPOINTS.register, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    username,
                    email,
                    password,
                    confirmPassword
                })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Erreur lors de l'inscription");
            }

            const data = await response.json();
            this.setSession(data);
            return data;
        } catch (error) {
            throw error;
        }
    }

    // Se connecter avec un compte existant
    async login(username, password) {
        try {
            const response = await fetch(API_ENDPOINTS.login, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    username,
                    password
                })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Erreur lors de la connexion");
            }

            const data = await response.json();
            this.setSession(data);
            return data;
        } catch (error) {
            throw error;
        }
    }

    // Rafraîchir le token JWT
    async refreshTokenFunc() {
        if (!this.refreshToken) {
            return false;
        }

        try {
            const response = await fetch(API_ENDPOINTS.refreshToken, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(this.refreshToken)
            });

            if (!response.ok) {
                this.logout();
                return false;
            }

            const data = await response.json();
            this.setSession(data);
            return true;
        } catch (error) {
            this.logout();
            return false;
        }
    }

    // Se déconnecter
    logout() {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        localStorage.removeItem('tokenExpiration');
        this.token = null;
        this.refreshToken = null;
        this.user = null;
        this.tokenExpiration = null;
    }

    // Stocker les informations de session
    setSession(data) {
        this.token = data.token;
        this.refreshToken = data.refreshToken;
        this.user = data.user;
        this.tokenExpiration = data.expiration;
        
        localStorage.setItem('token', data.token);
        localStorage.setItem('refreshToken', data.refreshToken);
        localStorage.setItem('user', JSON.stringify(data.user));
        localStorage.setItem('tokenExpiration', data.expiration);
    }

    // Obtenir l'en-tête d'autorisation
    getAuthHeader() {
        return {
            'Authorization': `Bearer ${this.token}`
        };
    }
}

// Gestion des sessions de concentration
class SessionService {
    constructor(authService) {
        this.authService = authService;
    }

    // Obtenir toutes les sessions
    async getAllSessions() {
        try {
            const response = await fetch(API_ENDPOINTS.sessions, {
                method: 'GET',
                headers: {
                    ...this.authService.getAuthHeader(),
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error("Impossible de récupérer les sessions");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    // Créer une nouvelle session
    async createSession(name, description, plannedDurationMinutes) {
        try {
            const response = await fetch(API_ENDPOINTS.sessions, {
                method: 'POST',
                headers: {
                    ...this.authService.getAuthHeader(),
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    name,
                    description,
                    plannedDurationMinutes
                })
            });

            if (!response.ok) {
                throw new Error("Impossible de créer la session");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    // Démarrer une session
    async startSession(sessionId) {
        try {
            const response = await fetch(`${API_ENDPOINTS.sessions}/${sessionId}/start`, {
                method: 'POST',
                headers: {
                    ...this.authService.getAuthHeader(),
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error("Impossible de démarrer la session");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    // Terminer une session
    async endSession(sessionId, notes) {
        try {
            const response = await fetch(`${API_ENDPOINTS.sessions}/${sessionId}/end`, {
                method: 'POST',
                headers: {
                    ...this.authService.getAuthHeader(),
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    notes
                })
            });

            if (!response.ok) {
                throw new Error("Impossible de terminer la session");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    // Supprimer une session
    async deleteSession(sessionId) {
        try {
            const response = await fetch(`${API_ENDPOINTS.sessions}/${sessionId}`, {
                method: 'DELETE',
                headers: this.authService.getAuthHeader()
            });

            if (!response.ok) {
                throw new Error("Impossible de supprimer la session");
            }

            return true;
        } catch (error) {
            throw error;
        }
    }
}

// Gestion de la liste noire
class BlacklistService {
    constructor(authService) {
        this.authService = authService;
    }

    // Obtenir tous les éléments de la liste noire
    async getAllBlacklistItems() {
        try {
            const response = await fetch(API_ENDPOINTS.blacklist, {
                method: 'GET',
                headers: {
                    ...this.authService.getAuthHeader(),
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error("Impossible de récupérer les éléments de la liste noire");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    // Ajouter un élément à la liste noire
    async addBlacklistItem(url, name, reason) {
        try {
            const response = await fetch(API_ENDPOINTS.blacklist, {
                method: 'POST',
                headers: {
                    ...this.authService.getAuthHeader(),
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    url,
                    name,
                    reason
                })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "Impossible d'ajouter l'élément à la liste noire");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    // Supprimer un élément de la liste noire
    async deleteBlacklistItem(itemId) {
        try {
            const response = await fetch(`${API_ENDPOINTS.blacklist}/${itemId}`, {
                method: 'DELETE',
                headers: this.authService.getAuthHeader()
            });

            if (!response.ok) {
                throw new Error("Impossible de supprimer l'élément de la liste noire");
            }

            return true;
        } catch (error) {
            throw error;
        }
    }

    // Vérifier si une URL est bloquée
    async isUrlBlacklisted(url) {
        try {
            const response = await fetch(`${API_ENDPOINTS.blacklist}/check?url=${encodeURIComponent(url)}`, {
                method: 'GET',
                headers: this.authService.getAuthHeader()
            });

            if (!response.ok) {
                throw new Error("Impossible de vérifier l'URL");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }
}

// Gestion des statistiques et motivation
class StatsService {
    constructor(authService) {
        this.authService = authService;
    }

    // Obtenir les statistiques générales
    async getStats() {
        try {
            const response = await fetch(API_ENDPOINTS.stats, {
                method: 'GET',
                headers: {
                    ...this.authService.getAuthHeader(),
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error("Impossible de récupérer les statistiques");
            }

            return await response.json();
        } catch (error) {
            throw error;
        }
    }

    // Obtenir une citation motivante
    async getRandomQuote() {
        try {
            const response = await fetch(API_ENDPOINTS.quote, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error("Impossible de récupérer une citation");
            }

            const data = await response.json();
            return data.quote;
        } catch (error) {
            return "La productivité n'est pas un accident. C'est toujours le résultat d'un engagement envers l'excellence.";
        }
    }

    // Obtenir un message motivant basé sur le nombre de sessions
    async getMotivationalMessage(sessionCount) {
        try {
            const response = await fetch(`${API_ENDPOINTS.message}?sessionCount=${sessionCount}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error("Impossible de récupérer un message motivant");
            }

            const data = await response.json();
            return data.message;
        } catch (error) {
            return "Continuez comme ça, vous faites des progrès!";
        }
    }
}

// Exportation des services
const authService = new AuthService();
const sessionService = new SessionService(authService);
const blacklistService = new BlacklistService(authService);
const statsService = new StatsService(authService);
