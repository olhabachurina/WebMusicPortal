﻿document.addEventListener("DOMContentLoaded", () => {
    const songsLink = document.getElementById("songsLink");
    const usersLink = document.getElementById("usersLink");
    const genresLink = document.getElementById("genresLink");

    const songsSection = document.getElementById("songsSection");
    const usersSection = document.getElementById("usersSection");
    const genresSection = document.getElementById("genresSection");

    songsLink.addEventListener("click", (e) => {
        e.preventDefault();
        loadSongs();
        showSection(songsSection);
    });

    usersLink.addEventListener("click", (e) => {
        e.preventDefault();
        loadUsers();
        showSection(usersSection);
    });

    genresLink.addEventListener("click", (e) => {
        e.preventDefault();
        loadGenres();
        showSection(genresSection);
    });

    function showSection(section) {
        songsSection.style.display = "none";
        usersSection.style.display = "none";
        genresSection.style.display = "none";
        section.style.display = "block";
    }

    // CRUD для песен
    function loadSongs() {
        fetch("/api/songs")
            .then(response => response.json())
            .then(data => populateSongs(data))
            .catch(error => console.error("Ошибка загрузки песен:", error));
    }

    function populateSongs(songs) {
        const songsList = document.getElementById("songsList");
        if (!songsList) {
            console.error("Элемент #songsList не найден в DOM.");
            return;
        }

        let html = "";
        songs.forEach(song => {
            html += `
                <li>
                    ${song.title} - ${song.artist} (${song.genre})
                    <button onclick="editSong(${song.songId})">Редактировать</button>
                    <button onclick="deleteSong(${song.songId})">Удалить</button>
                </li>`;
        });
        songsList.innerHTML = html;
    }

    window.editSong = function (songId) {
        fetch(`/api/songs/${songId}`)
            .then(response => response.json())
            .then(song => {
                document.getElementById("songId").value = song.songId;
                document.getElementById("title").value = song.title;
                document.getElementById("artist").value = song.artist;
                document.getElementById("genreId").value = song.genreId;
                document.getElementById("mood").value = song.mood || '';
                
                document.getElementById("musicFile").value = '';
                document.getElementById("videoFile").value = '';
            })
            .catch(error => console.error("Ошибка загрузки песни:", error));
    }

    window.deleteSong = function (songId) {
        if (confirm("Вы уверены, что хотите удалить эту песню?")) {
            fetch(`/api/songs/${songId}`, {
                method: "DELETE"
            })
                .then(() => {
                    loadSongs();
                    alert("Песня успешно удалена");
                })
                .catch(error => console.error("Ошибка удаления песни:", error));
        }
    }

    document.getElementById("songForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const musicFile = document.getElementById("musicFile").files[0];
        const videoFile = document.getElementById("videoFile").files[0];

        if (musicFile && musicFile.size > 30 * 1024 * 1024) {
            alert("Файл музыки слишком большой. Максимальный размер 30 MB.");
            return;
        }

        if (videoFile && videoFile.size > 30 * 1024 * 1024) {
            alert("Файл видео слишком большой. Максимальный размер 30 MB.");
            return;
        }

        const formData = new FormData(this);
        const songId = document.getElementById("songId").value;

        if (songId) {
            updateSong(songId, formData);
        } else {
            createSong(formData);
        }
    });

    function createSong(formData) {
        fetch("/api/songs", {
            method: "POST",
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorText => {
                        throw new Error(`Ошибка: ${response.status} ${response.statusText} - ${errorText}`);
                    });
                }
                return response.json();
            })
            .then(() => {
                loadSongs();
                clearSongForm();
                alert("Песня успешно добавлена");
            })
            .catch(error => console.error("Ошибка создания песни:", error));
    }

    function updateSong(songId, formData) {
        fetch(`/api/songs/${songId}`, {
            method: "PUT",
            body: formData
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorText => {
                        throw new Error(`Ошибка: ${response.status} ${response.statusText} - ${errorText}`);
                    });
                }
                return response.json();
            })
            .then(() => {
                loadSongs();
                clearSongForm();
                alert("Песня успешно обновлена");
            })
            .catch(error => console.error("Ошибка обновления песни:", error));
    }

    function clearSongForm() {
        document.getElementById("songId").value = "";
        document.getElementById("title").value = "";
        document.getElementById("artist").value = "";
        document.getElementById("genreId").value = "";
        document.getElementById("mood").value = "";
        document.getElementById("musicFile").value = "";
        document.getElementById("videoFile").value = "";
    }

    // CRUD для пользователей
    function loadUsers() {
        fetch("/api/users")
            .then(response => response.json())
            .then(data => populateUsers(data))
            .catch(error => console.error("Ошибка загрузки пользователей:", error));
    }

    function populateUsers(users) {
        const usersList = document.getElementById("usersList");
        let html = "";
        users.forEach(user => {
            html += `
                <li>
                    ${user.userName} (${user.email}) - ${user.role}
                    <button onclick="editUser(${user.userId})">Редактировать</button>
                    <button onclick="deleteUser(${user.userId})">Удалить</button>
                </li>`;
        });
        usersList.innerHTML = html;
    }

    window.editUser = function (userId) {
        fetch(`/api/users/${userId}`)
            .then(response => response.json())
            .then(user => {
               
                const userIdInput = document.getElementById("userId");
                const userNameInput = document.getElementById("userName");
                const emailInput = document.getElementById("email");
                const roleInput = document.getElementById("role");
                const passwordInput = document.getElementById("password"); 

                if (userIdInput) userIdInput.value = user.userId || '';
                if (userNameInput) userNameInput.value = user.userName || '';
                if (emailInput) emailInput.value = user.email || '';
                if (roleInput) roleInput.value = user.role || '';
                if (passwordInput) passwordInput.value = ''; // Очищаем поле пароля при редактировании
            })
            .catch(error => console.error("Ошибка загрузки пользователя:", error));
    }

    window.deleteUser = function (userId) {
        if (confirm("Вы уверены, что хотите удалить этого пользователя?")) {
            fetch(`/api/users/${userId}`, {
                method: "DELETE"
            })
                .then(() => {
                    loadUsers();
                    alert("Пользователь успешно удален");
                })
                .catch(error => console.error("Ошибка удаления пользователя:", error));
        }
    }

    document.getElementById("userForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const userId = document.getElementById("userId").value;
        const userName = document.getElementById("userName").value;
        const email = document.getElementById("email").value;
        const role = document.getElementById("role").value;
        const password = document.getElementById("password").value;

        if (!userId && !password) {
            alert("Пароль обязателен для создания нового пользователя.");
            return;
        }

        const userData = {
            userName: userName,
            email: email,
            role: role
        };

        if (password) {
            userData.password = password;
        }

        if (userId) {
            updateUser(userId, userData);
        } else {
            createUser(userData);
        }
    });

    function createUser(userData) {
        fetch("/api/users", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(userData)
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(error => { throw new Error(error.message); });
                }
                return response.json();
            })
            .then(() => {
                loadUsers();
                clearUserForm();
                alert("Пользователь успешно добавлен");
            })
            .catch(error => console.error("Ошибка создания пользователя:", error));
    }

    function updateUser(userId, userData) {
        fetch(`/api/users/${userId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ ...userData, userId })
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorMessage => {
                        throw new Error(errorMessage);
                    });
                }
                return response.text().then(text => text ? JSON.parse(text) : {});
            })
            .then(() => {
                loadUsers();
                clearUserForm();
                alert("Пользователь успешно обновлен");
            })
            .catch(error => console.error("Ошибка обновления пользователя:", error.message));
    }

    function clearUserForm() {
        document.getElementById("userId").value = "";
        document.getElementById("userName").value = "";
        document.getElementById("email").value = "";
        document.getElementById("password").value = "";
        document.getElementById("role").value = "Пользователь";
    }

    // CRUD для жанров
    function loadGenres() {
        fetch("/api/genres")
            .then(response => response.json())
            .then(data => populateGenresDropdown(data))
            .catch(error => console.error("Ошибка загрузки жанров:", error));
    }

    function populateGenresDropdown(genres) {
        const genreSelect = document.getElementById("genreId");
        genreSelect.innerHTML = '<option value="">Выберите жанр</option>'; 

        genres.forEach(genre => {
            let option = document.createElement("option");
            option.value = genre.genreId;
            option.text = genre.name;
            genreSelect.appendChild(option);
        });

      
        const genresList = document.getElementById("genresList");
        if (genresList) {
            genresList.innerHTML = '';
            genres.forEach(genre => {
                genresList.innerHTML += `
                    <li>
                        ${genre.name}
                        <button onclick="editGenre(${genre.genreId})">Редактировать</button>
                        <button onclick="deleteGenre(${genre.genreId})">Удалить</button>
                    </li>`;
            });
        }
    }

    window.editGenre = function (genreId) {
        fetch(`/api/genres/${genreId}`)
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorMessage => {
                        throw new Error(errorMessage);
                    });
                }
                return response.json();
            })
            .then(genre => {
                document.getElementById("genreId").value = genre.genreId;
                document.getElementById("genreName").value = genre.name;
            })
            .catch(error => console.error("Ошибка загрузки жанра:", error));
    }

    window.deleteGenre = function (genreId) {
        if (confirm("Вы уверены, что хотите удалить этот жанр?")) {
            fetch(`/api/genres/${genreId}`, {
                method: "DELETE"
            })
                .then(() => {
                    loadGenres();
                    alert("Жанр успешно удален");
                })
                .catch(error => console.error("Ошибка удаления жанра:", error));
        }
    }

    document.getElementById("genreForm").addEventListener("submit", function (e) {
        e.preventDefault();
        const genreId = document.getElementById("genreId").value;
        const genreData = {
            genreId: genreId,
            name: document.getElementById("genreName").value
        };

        if (genreId) {
            updateGenre(genreId, genreData);
        } else {
            createGenre(genreData);
        }
    });

    function createGenre(genreData) {
        delete genreData.genreId;

        fetch("/api/genres", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(genreData)
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorMessage => {
                        throw new Error(errorMessage);
                    });
                }
                return response.json();
            })
            .then(() => {
                loadGenres();
                clearGenreForm();
                alert("Жанр успешно добавлен");
            })
            .catch(error => console.error("Ошибка создания жанра:", error.message));
    }

    function updateGenre(genreId, genreData) {
        fetch(`/api/genres/${genreId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(genreData)
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(errorMessage => {
                        throw new Error(errorMessage);
                    });
                }
                return response.json();
            })
            .then(() => {
                loadGenres();
                clearGenreForm();
                alert("Жанр успешно обновлен");
            })
            .catch(error => console.error("Ошибка обновления жанра:", error.message));
    }

    function clearGenreForm() {
        document.getElementById("genreId").value = "";
        document.getElementById("genreName").value = "";
    }

    // Загрузить данные при загрузке страницы
    loadSongs();
    loadUsers();
    loadGenres();
});