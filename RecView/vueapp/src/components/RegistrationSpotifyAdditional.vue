<template>
  <div class="block">
    <div class="columns is-mobile is-centered">
      <div class="column is-half">
        <div class="card">
          <div class="field">
            <div class="title"><strong>Registration</strong></div>
            <div class="field">
              <label class="label">Firstname</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.info.firstname"
                    @input="setupState.info.firstname = $event.target.value"
                    class="input"
                    type="text"
                    placeholder="e.g. Alex"
                  />
                  <span class="icon is-small is-left">
                    <i class="fa-solid fa-f"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.info.firstname.$error"
                >
                  {{v$.info.firstname.$errors[0].$message}}
                </p>
              </div>
            </div>
            <div class="field">
              <label class="label">Lastname</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.info.lastname"
                    @input="setupState.info.lastname = $event.target.value"
                    class="input"
                    type="text"
                    placeholder="e.g. Romanov"
                  />
                  <span class="icon is-small is-left">
                    <i class="fa-solid fa-l"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.info.lastname.$error"
                >
                  {{v$.info.lastname.$errors[0].$message}}
                </p>
              </div>
            </div>
            <div class="field">
              <p class="control">
                <button
                  class="button is-success"
                  @click="fetchUsers"
                >
                  <strong>Create account</strong>
                </button>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import axios from "axios";
import { computed, reactive, onMounted } from "vue";
import useVuelidate from "@vuelidate/core";
import { required, minLength, email } from "@vuelidate/validators";
import { useStore } from "vuex";
import { useRouter } from "vue-router";

export default {
  components: {},

  setup() {
    const store = useStore();
    const router = useRouter();

    const setupState = reactive({
      info: {
        email: "",
        username: "",
        firstname: "",
        lastname: "",
        country: "",
        spotifyId: "",
        password: "",
      },
    });
    const rules = computed(() => {
      return {
        info: {
          email: { required, email },
          firstname: { minLength: minLength(3), required },
          lastname: { minLength: minLength(3), required },
          password: { required, minLength: minLength(8) },
        },
      };
    });

    const v$ = useVuelidate(rules, setupState);

    const password = reactive({
      value: "", // Хранение пароля в реактивном объекте
    });

    const generatePassword = () => {
      const length = 12; // Длина пароля
      const charset =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+[]{}|;:,.<>?";
      let newPassword = "";
      for (let i = 0, n = charset.length; i < length; ++i) {
        newPassword += charset.charAt(Math.floor(Math.random() * n));
      }
      password.value = newPassword;
      setupState.info.password = newPassword; // Сохранение пароля в состоянии
      // Сохранение пароля в реактивном объекте
    };

    const fetchUsers = async () => {
      try {
        await v$.value.$validate(); // Проверка валидации
        if (!v$.value.$pending && !v$.value.$error) {
          // Проверка валидации
          const response = await axios.post(
            "https://localhost:7154/api/SpotifyAuth/register",
            setupState.info,
            {
              headers: {
                "Content-Type": "application/json",
                accept: "text/plain",
              },
            }
          );
          console.log(response);
          const token = response.data; // Извлечение токена из ответа
          localStorage.setItem("token", token);
          // Для использования Vuex и Vue Router
          store.dispatch("setUser");
          router.push("/MainPage");
        }
      } catch (e) {
        console.error(e);
        alert(`Error: ${e.message}`);
      }
    };

    onMounted(() => {
      const url = new URL(window.location.href);
      const params = new URLSearchParams(url.search);

      setupState.info.email = params.get("email");
      setupState.info.username = params.get("displayName");
      setupState.info.country = params.get("country");
      setupState.info.spotifyId = params.get("userId");
      generatePassword();
    });

    return {
      setupState,
      v$,
      generatePassword,
      fetchUsers,
    };
  },
};
</script>
  
  <style scoped>
.card {
  padding: 3rem;
}
</style>