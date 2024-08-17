<template>
  <div class="block">
    <div class="columns is-mobile is-centered">
      <div class="column is-half">
        <div class="card">
          <div class="field">
            <div class="title"><strong>Log In</strong></div>
            <p class="control has-icons-left has-icons-right">
              <input
                v-bind:value="user.email"
                @input="user.email = $event.target.value"
                class="input"
                type="email"
                placeholder="Email"
              />
              <span class="icon is-small is-left">
                <i class="fas fa-envelope"></i>
              </span>
              <span class="icon is-small is-right">
                <i class="fas fa-check"></i>
              </span>
            </p>
          </div>
          <div class="field">
            <p class="control has-icons-left">
              <input
                v-bind:value="user.password"
                @input="user.password = $event.target.value"
                class="input"
                type="password"
                placeholder="Password"
              />
              <span class="icon is-small is-left">
                <i class="fas fa-lock"></i>
              </span>
            </p>
          </div>
          <div class="field">
            <p class="control">
              <button
                class="button is-success"
                @click="login"
              >
                <strong>Log In</strong>
              </button>
            </p>
            <div class="
              noSignUp">
              Don't have an account yet? <router-link
                class="button is-info is-inverted is-small"
                to="/Signup"
              >Sign up</router-link>
            </div>
          </div>
          <button
            class="button is-primary"
            @click="loginSpotify"
          >
            <span class="icon">
              <i class="fab fa-spotify"></i>
            </span>
            <span>Log In through Spotify</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
  
  <script>
import axios from "axios";
import Registration from "./SignUp.vue";

export default {
  components: { Registration },
  data() {
    return {
      errorVisible: false,
      user: {
        email: "",
        password: "",
      },
    };
  },

  mounted() {
    console.log("mounted");
    localStorage.removeItem("token");
    this.$store.dispatch("logout");
  },

  methods: {
    async login() {
      const data = this.user;
      console.log(data);
      try {
        // Выполнение запроса с помощью Axios
        const response = await axios.post(
          "https://localhost:7154/api/Auth/login",
          data,
          {
            headers: {
              "Content-Type": "application/json",
              accept: "text/plain",
            },
          }
        );

        console.log(response);
        if (response && response.data) {
          localStorage.setItem("token", response.data.token); // Предполагаем, что сервер возвращает объект с токеном

          // Обновление состояния пользователя в Vuex
          await this.$store.dispatch("setUser");

          // Перенаправление на страницу
          this.$router.push("/MainPage");
        } else {
          console.error("Response does not contain data:", response);
        }
      } catch (error) {
        console.error("Login failed:", error);
        // Дополнительные действия при ошибке (например, показать сообщение об ошибке)
      }
    },

    async loginSpotify() {
      try {
        // Redirect to Spotify's login page
        window.location.href = "https://localhost:7154/api/SpotifyAuth/login";

        //this.store.dispatch("handleSpotifyCallback");
      } catch (error) {
        console.error(
          "Error occurred while logging in through Spotify:",
          error
        );
      }
    },
  },
};
</script>
  
  <style scoped>
.card {
  padding: 3rem;
}
</style>