<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newUserRule"
        :editRule="userRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('Public.UserName')" prop="userName">
                        <Input v-model="slotProps.createModel.userName" :maxlength="32" :minlength="2"></Input>
                    </FormItem>
                    <FormItem :label="$t('Public.Name')" prop="name">
                        <Input v-model="slotProps.createModel.name" :maxlength="32"></Input>
                    </FormItem>
                    <FormItem :label="$t('Users.Surname')" prop="surname">
                        <Input v-model="slotProps.createModel.surname" :maxlength="1024"></Input>
                    </FormItem>
                    <FormItem :label="$t('Users.EmailAddress')" prop="emailAddress">
                        <Input v-model="slotProps.createModel.emailAddress" type="email" :maxlength="32"></Input>
                    </FormItem>
                    <FormItem :label="$t('Users.Password')" prop="password">
                        <Input v-model="slotProps.createModel.password" type="password" :maxlength="32"></Input>
                    </FormItem>
                    <FormItem :label="$t('Users.ConfirmPassword')" prop="confirmPassword">
                        <Input v-model="slotProps.createModel.confirmPassword" type="password" :maxlength="32"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.createModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                </TabPane>
                <TabPane :label="$t('Users.UserRoles')" name="roles">
                    <CheckboxGroup v-model="slotProps.createModel.roleNames">
                        <Checkbox :label="role.normalizedName" v-for="role in roles" :key="role.id"><span>{{role.name}}</span></Checkbox>
                    </CheckboxGroup>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="$t('Public.Details')" name="detail">
                    <FormItem :label="$t('Public.UserName')" prop="userName">
                        <Input v-model="slotProps.editModel.userName" :maxlength="32" :minlength="2"></Input>
                    </FormItem>
                    <FormItem :label="$t('Public.Name')" prop="name">
                        <Input v-model="slotProps.editModel.name" :maxlength="32"></Input>
                    </FormItem>
                    <FormItem :label="$t('Users.Surname')" prop="surname">
                        <Input v-model="slotProps.editModel.surname" :maxlength="1024"></Input>
                    </FormItem>
                    <FormItem :label="$t('Users.EmailAddress')" prop="emailAddress">
                        <Input v-model="slotProps.editModel.emailAddress" type="email" :maxlength="32"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{$t('Public.IsActive')}}</Checkbox>
                    </FormItem>
                </TabPane>
                <TabPane :label="$t('Users.UserRoles')" name="roles">
                    <CheckboxGroup v-model="slotProps.editModel.roleNames">
                        <Checkbox :label="role.normalizedName" v-for="role in roles" :key="role.id"><span>{{role.name}}</span></Checkbox>
                    </CheckboxGroup>
                </TabPane>
            </Tabs>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import UserApi from "@/api/user";

export default {
  components: {
    simplePage
  },
  async created() {
    let rep = await UserApi.GetAllRoles();
    this.allRoles = rep.data.result.items;
  },
  computed: {
    roles() {
      return this.allRoles;
    }
  },
  data() {
    var _vm = this;
    const validatePassCheck = (rule, value, callback) => {
      if (!value) {
        callback(new Error("Please enter your password again"));
      } else if (value !== _vm.$refs.simplepage.createModel.password) {
        callback(new Error("The two input passwords do not match!"));
      } else {
        callback();
      }
    };
    return {
      title: "Menu.Pages.Users",
      api: UserApi,
      allRoles: [],
      newUserRule: {
        userName: [{ required: true, trigger: "blur" }],
        name: [{ required: true, trigger: "blur" }],
        surname: [{ required: true, trigger: "blur" }],
        emailAddress: [{ required: true, trigger: "blur" }, { type: "email" }],
        password: [{ required: true, trigger: "blur" }],
        confirmPassword: { validator: validatePassCheck, trigger: "blur" }
      },
      userRule: {
        userName: [{ required: true, trigger: "blur" }],
        name: [{ required: true, trigger: "blur" }],
        surname: [{ required: true, trigger: "blur" }],
        emailAddress: [{ required: true, trigger: "blur" }, { type: "email" }]
      },
      columnsetting: {
        actionOption: {
            edit: true,
            delete: true
        },
        columns: [
          {
            title: this.$t("Public.UserName"),
            key: "userName"
          },
          {
            title: this.$t("Users.FullName"),
            key: "fullName"
          },
          {
            title: this.$t("Users.EmailAddress"),
            key: "emailAddress"
          },
          {
            title: this.$t("Public.IsActive"),
            render: (h, params) => {
              return h("Checkbox", {
                props: {
                  value: params.row.isActive,
                  disabled: true
                }
              });
            }
          }
        ]
      }
    };
  }
};
</script>