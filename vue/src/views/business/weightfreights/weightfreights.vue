<template>
    <simplePage ref="simplepage" :title="title" 
        :columnsetting="columnsetting" 
        :api="api"
        :newRule="newWeightFreightRule"
        :editRule="weightFreightRule">
        <template slot="newform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('StartingWeight')" prop="startingWeight">
                        <Input v-model.number="slotProps.createModel.startingWeight"></Input>
                    </FormItem>
                    <FormItem :label="L('StartingPrice')" prop="startingPrice">
                        <Input v-model.number="slotProps.createModel.startingPrice"></Input>
                    </FormItem>
                    <FormItem :label="L('StepWeight')" prop="stepWeight">
                        <Input v-model.number="slotProps.createModel.stepWeight"></Input>
                    </FormItem>
                    <FormItem :label="L('Price')" prop="price">
                        <Input v-model.number="slotProps.createModel.price"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticLine')" prop="logisticLineId">
                        <Input v-model="slotProps.createModel.logisticLineId"></Input>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
        <template slot="editform" slot-scope="slotProps">
            <Tabs value="detail">
                <TabPane :label="L('Details')" name="detail">
                    <FormItem :label="L('StartingWeight')" prop="startingWeight">
                        <Input v-model.number="slotProps.editModel.startingWeight"></Input>
                    </FormItem>
                    <FormItem :label="L('StartingPrice')" prop="startingPrice">
                        <Input v-model.number="slotProps.editModel.startingPrice"></Input>
                    </FormItem>
                    <FormItem :label="L('StepWeight')" prop="stepWeight">
                        <Input v-model.number="slotProps.editModel.stepWeight"></Input>
                    </FormItem>
                    <FormItem :label="L('Price')" prop="price">
                        <Input v-model.number="slotProps.editModel.price"></Input>
                    </FormItem>
                    <FormItem :label="L('LogisticLine')" prop="logisticLineId">
                        <Input v-model="slotProps.editModel.logisticLineId"></Input>
                    </FormItem>
                    <FormItem>
                        <Checkbox v-model="slotProps.editModel.isActive">{{'IsActive'|l}}</Checkbox>
                    </FormItem>
                </TabPane>
            </Tabs>
        </template>
    </simplePage>
</template>
<script>
import simplePage from "../../../components/simplepage.vue";
import WeightFreightApi from "@/api/weightFreight";

export default {
  components: {
    simplePage
  },
  data() {
    return {
      title: "WeightFreight",
      api: WeightFreightApi,
      newWeightFreightRule: {
        startingWeight: [{ type: "number" }],
        startingPrice: [{ type: "number" }],
        stepWeight: [{ type: "number" }],
        price: [{ type: "number" }],
        logisticLineId: [{ type: "number" }]
      },
      weightFreightRule: {
        startingWeight: [{ type: "number" }],
        startingPrice: [{ type: "number" }],
        stepWeight: [{ type: "number" }],
        price: [{ type: "number" }],
        logisticLineId: [{ type: "number" }]
      },
      columnsetting: {
        needAction: true,
        columns: [
          {
            title: this.L("StartingWeight"),
            key: "startingWeight"
          },
          {
            title: this.L("StartingPrice"),
            key: "startingPrice"
          },
          {
            title: this.L("StepWeight"),
            key: "StepWeight"
          },
          {
            title: this.L("Price"),
            key: "price"
          },
          {
            title: this.L("LogisticLineId"),
            key: "logisticLineId"
          },
          {
            title: this.L("IsActive"),
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